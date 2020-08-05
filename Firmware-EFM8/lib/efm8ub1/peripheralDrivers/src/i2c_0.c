/**************************************************************************//**
 * Copyright (c) 2015 by Silicon Laboratories Inc. All rights reserved.
 *
 * http://developer.silabs.com/legal/version/v11/Silicon_Labs_Software_License_Agreement.txt
 *****************************************************************************/

#include "i2c_0.h"
#include "assert.h"

#if EFM8PDL_I2C0_AUTO_PAGE == 1
// declare variable needed for autopage enter/exit
#define DECL_PAGE uint8_t savedPage
// enter autopage section
#define SET_PAGE(p)     do                                                    \
                        {                                                     \
                          savedPage = SFRPAGE;  /* save current SFR page */   \
                          SFRPAGE = (p);        /* set SFR page */            \
                        } while(0)
// exit autopage section
#define RESTORE_PAGE    do                                                    \
                        {                                                     \
                          SFRPAGE = savedPage;  /* restore saved SFR page */  \
                        } while(0)

#else
#define DECL_PAGE
#define SET_PAGE
#define RESTORE_PAGE
#endif

#define ALL_FLAGS  I2C0_TXMODE_SF \
                   | I2C0_START_SF \
                   | I2C0_STOP_SF \
                   | I2C0_ACKREQ_SF \
                   | I2C0_ARBLOST_SF \
                   | I2C0_ACK_SF

bool I2C0_getIntFlag()
{
  bool val;
  DECL_PAGE;
  SET_PAGE(0x00);
  val = SMB0CN0_SI;
  RESTORE_PAGE;
  return val;
}

void I2C0_clearIntFlag()
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CN0_SI = 0;
  RESTORE_PAGE;
}

uint8_t I2C0_getStatusFlag(uint8_t flag) reentrant
{
  uint8_t val;
  DECL_PAGE;
  SET_PAGE(0x00);
  val = SMB0CN0 & flag;
  RESTORE_PAGE;
  return val;
}

void I2C0_setStatusFlag(uint8_t flag, uint8_t state) reentrant
{
  DECL_PAGE;
  SET_PAGE(0x00);
  if (state)
  {
    SMB0CN0 |= flag;
  }
  else
  {
    SMB0CN0 &= ~flag;
  }
  RESTORE_PAGE;
}

uint8_t I2C0_read()
{
  uint8_t val;
  DECL_PAGE;
  SET_PAGE(0x00);
  val = SMB0DAT;
  RESTORE_PAGE;
  return val;
}

void I2C0_write(uint8_t value)
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0DAT = value;
  RESTORE_PAGE;
}

void I2C0_abort()
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CF &= ~SMB0CF_ENSMB__BMASK;
  SMB0CF |= SMB0CF_ENSMB__BMASK;
  SMB0CN0 &= ~(SMB0CN0_STA__BMASK
      | SMB0CN0_STO__BMASK
      | SMB0CN0_ACKRQ__BMASK);
  RESTORE_PAGE;
}

void I2C0_ack(bool ack)
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CN0_ACK = ack;
  RESTORE_PAGE;
}

void I2C0_start()
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CN0_STA = 1;
  RESTORE_PAGE;
}

void I2C0_stop()
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CN0_STO = 1;
  RESTORE_PAGE;
}

void I2C0_init(I2C0_Timebase_t timebase, bool sclLow)
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CF &= ~(SMB0CF_SMBCS__FMASK | SMB0CF_SMBTOE__BMASK);
  SMB0CF |= timebase
            | SMB0CF_ENSMB__ENABLED
            | SMB0CF_INH__SLAVE_DISABLED
            | SMB0CF_SMBFTE__FREE_TO_ENABLED
            | (uint8_t) sclLow << SMB0CF_SMBTOE__SHIFT;
  SMB0ADM |= SMB0ADM_EHACK__ADR_ACK_AUTOMATIC;
  RESTORE_PAGE;
}

void I2C0_initSlaveAddress(uint8_t address, uint8_t mask)
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CF  &= ~SMB0CF_INH__BMASK;
  SMB0ADR = address;
  SMB0ADM = mask | SMB0ADM_EHACK__BMASK;
  RESTORE_PAGE;
}

void I2C0_reset() {
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CF  = 0x0;
  SMB0TC  = 0x0;
  SMB0CN0  = 0x0;
  SMB0ADR = 0x0;
  SMB0ADM = 0x0;
  RESTORE_PAGE;
}

#if EFM8PDL_I2C0_USE_STATEMACHINE == 1
SI_INTERRUPT_PROTO(I2C0_ISR, SMBUS0_IRQn);
SI_INTERRUPT(I2C0_ISR, SMBUS0_IRQn)
{
  SFRPAGE = 0x0; //Rely on page stack for restore

  if(SMB0CN0_ARBLOST) {
    I2C0_arbLostCb(SMB0CN0 & 0xF0);
  }

  // Normal operation
  switch (SMB0CN0 & 0xF0)// Status vector
  {
    // SMB0CN0_MASTER ---------------------------------------------------------------

    // Master Transmitter/Receiver: START condition transmitted.
    case I2C0_MASTER_START:
      I2C0_startCb();
      SMB0CN0_STA = 0;
      break;

    // Master Transmitter: Data byte transmitted
    case I2C0_MASTER_TXDATA:
      I2C0_TxDataCb(true, SMB0CN0_ACK);
      break;

    // Master Receiver: byte received
    case I2C0_MASTER_RXDATA:
      I2C0_RxDataCb(true);
      break;

    case I2C0_SLAVE_TXDATA:
      I2C0_TxDataCb(false, SMB0CN0_ACK);
      break;

    case I2C0_SLAVE_ADDRESS:
      if(SMB0CN0_ARBLOST)
      {
        I2C0_arbLostCb(SMB0CN0 & 0xF0);
      }
      else
      {
        I2C0_addressCb();
      }
      SMB0CN0_STA = 0;  // Clear SMB0CN0_STA bit
      break;
                                      // Slave Receiver: Data received
    case I2C0_SLAVE_RXDATA:
      I2C0_RxDataCb(false);
      break;

                                      // Slave Receiver: Stop received while either a Slave Receiver or Slave
                                      // Transmitter
    case I2C0_SLAVE_RX_STOP:
    case I2C0_SLAVE_TX_STOP:
      I2C0_stopCb();
      SMB0CN0_STO = 0;
      break;

    default:
      break;

  } // end switch

  //Need an SMBus Restart?
  SMB0CN0_SI = 0;// Clear interrupt flag
}
#endif //EFM8PDL_I2C0_USE_STATEMACHINE

#if EFM8PDL_I2C0_USE_BUFFER == 1

SI_SEGMENT_VARIABLE(mAddress, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(mTxCount, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(mRxCount, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE_SEGMENT_POINTER(mTxBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE,
    SI_SEG_XDATA);
SI_SEGMENT_VARIABLE_SEGMENT_POINTER(mRxBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE,
    SI_SEG_XDATA);

SI_SEGMENT_VARIABLE(sRxSize, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(sRxCount, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(sTxCount, uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE_SEGMENT_POINTER(sRxBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE,
    SI_SEG_XDATA);
SI_SEGMENT_VARIABLE_SEGMENT_POINTER(sTxBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE,
    SI_SEG_XDATA);

SI_SEGMENT_VARIABLE(i2cBusy, bool, SI_SEG_IDATA);
SI_SEGMENT_VARIABLE(i2cReq, bool, SI_SEG_IDATA);
SI_SEGMENT_VARIABLE(i2cReceive, bool, SI_SEG_IDATA);
SI_SEGMENT_VARIABLE(mRetries, uint8_t, SI_SEG_XDATA);

void I2C0_transfer(uint8_t address,
    SI_VARIABLE_SEGMENT_POINTER(txBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE),
    SI_VARIABLE_SEGMENT_POINTER(rxBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE),
    uint8_t tx_len, uint8_t rx_len)
{
  DECL_PAGE;
  SET_PAGE(0x00);

  //Setup transfer
  mAddress = address;
  mTxBuffer = txBuffer;
  mRxBuffer = rxBuffer;
  mTxCount = tx_len;
  mRxCount = rx_len;
  mRetries = EFM8PDL_I2C0_MASTER_RETRIES;

  if (i2cBusy) {
    i2cReq = true;
  } else {
    //Start transfer
    SMB0CN0_STA = 1;
  }
  RESTORE_PAGE;
}

void I2C0_abortTransfer(){
  mTxCount = 0;
  mRxCount = 0;
  sTxCount = 0;
  
  i2cBusy = false;
  i2cReq = false;
}

uint8_t I2C0_txBytesRemaining() {
  return mTxCount;
}

uint8_t I2C0_rxBytesRemaining() {
  return mRxCount;
}

void I2C0_initSlave(uint8_t address,
                    SI_VARIABLE_SEGMENT_POINTER(commandBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE), uint8_t length)
{
  DECL_PAGE;
  SET_PAGE(0x00);
  SMB0CF  &= ~SMB0CF_INH__BMASK;
  SMB0ADR = address;
  SMB0ADM = 0xFF | SMB0ADM_EHACK__BMASK;
  sRxBuffer = commandBuffer;
  sRxSize = length;
  sRxCount = 0;
  RESTORE_PAGE;
}

uint8_t I2C0_getCommandLength() {
  return sRxCount;
}

void I2C0_sendResponse(
    SI_VARIABLE_SEGMENT_POINTER(dataBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE),
    uint8_t length) {
  sTxBuffer = dataBuffer;
  sTxCount = length;
}

SI_INTERRUPT_PROTO(I2C0_ISR, SMBUS0_IRQn);

SI_INTERRUPT(I2C0_ISR, SMBUS0_IRQn)
{
  SFRPAGE = 0x0;  // Rely on page stack to restore

  if(!SMB0CN0_MASTER)
  {
    /* HACK ALERT: if the bus is stuck low, the SMB0 peripheral will hang
     * until the bus clears. If additional USB packets come in while SMB0
     * is waiting, it appears to prevent the MASTER flag from getting set
     * (which should be done automatically once a START condition is set).
     * Until we figure out if this is a software bug or a chip errata, we'll
     * check to make sure we're in master mode (which we should always be),
     * and if not, abort. Otherwise, the below state machine gets screwed up
     * and we'll hang forever waiting for the commandComplete flag to be set.
     */
    I2C0_errorCb(I2C0_UNKNOWN_ERROR);
    I2C0_abort();
  }

  // Jump to status vector handler
  switch (SMB0CN0 & 0xF0)
  {
  // Master Transmitter/Receiver: START condition transmitted.
  case I2C0_MASTER_START:
    //Send address. If no tx data then initiate read.
    // Must make a single access to SMB0DAT because it's a fifo and
    // = follwed by |= writes to two slots in the fifo.
    if (!mTxCount)
    {
      SMB0DAT = (mAddress | 0x01);
      i2cReceive = true;
    }
    else
    {
      SMB0DAT = mAddress;
      i2cReceive = false;
    }
    //i2cReceive = SMB0DAT & 0x01;

    //tailchain: clear_start;
    SMB0CN0_STA = 0;
    i2cBusy = true;
    break;

  // Master Transmitter: Data byte transmitted
  case I2C0_MASTER_TXDATA:
    if (SMB0CN0_ACK)
    {
      if (i2cReceive)
      {
        //Handle read transfer
        if(!--mRxCount)
        {
          SMB0CN0_ACK = 0; //ack for first data receviced byte
        }
        //ACK = 1 by default so no need to set it
      }
      else
      {
        //Handle write's
        if (mTxCount)
        {
          //if data remains in tx buffer send next byte
          SMB0DAT = *mTxBuffer++;
          mTxCount--;
        }
        else
        {
          //Handle writes finished
          if (mRxCount)
          {
            //if tx done and rx remaining do repeated start
            SMB0CN0_STA = 1;
          }
          else
          {
            //if tx done and no rx then stop
            //tailchain: stop_seq
            I2C0_transferCompleteCb();
            SMB0CN0_STO = 1;
            SMB0CN0_STA = i2cReq;
            i2cReq = false;
            i2cBusy = false;
          } // if rxCount else
        }   //if txCount else
      }     //if ! rxReceive (rxReceive = ack of address so ignore)
    }       // if ACK
    else
    {
      //Error on NAC
      I2C0_errorCb(I2C0_NACK_ERROR);

      //tailchain: stop_seq
      SMB0CN0_STO = 1;
      SMB0CN0_STA = i2cReq; //necessary for tailchain
      i2cReq = false; //necessary for tailchain
      i2cBusy = false;
    }
    break;

  // Master Receiver: byte received
  case I2C0_MASTER_RXDATA:
    //Read in data we just received
    *mRxBuffer++ = SMB0DAT;

    //if bytes remain read this one and setup for next one
    if (mRxCount)
    {
      //NACK if the byte we are about to read is our last
      if (!--mRxCount)
      {
        SMB0CN0_ACK = 0;
      }
      //ACK =1  by default so no need to set it.
    }
    else
    {
      SMB0CN0_ACK = 0;

      // If no bytes remain notify user xfer complete and issue stop.
      //tailchain: stop_seq
      I2C0_transferCompleteCb();
      SMB0CN0_STO = 1;
      SMB0CN0_STA = i2cReq;
      i2cReq = false;
      i2cBusy = false;
    }
    break;

  // Slave Transmitter: Data byte transmitted
  case I2C0_SLAVE_TXDATA:
    if(SMB0CN0_ACK)
    {
      if(--sTxCount)
      {
        SMB0DAT = *sTxBuffer++;
      }
      else
      {
        I2C0_errorCb(I2C0_TXUNDER_ERROR);
      }
    }
    break;

  // Slave Receiver: Start+Address received
  case I2C0_SLAVE_ADDRESS:
    if(SMB0CN0_ARBLOST)
    {
      if(mRetries--)
      {
        i2cReq = true;
      }
      else
      {
        I2C0_errorCb(I2C0_ARBLOST_ERROR);
      }
    }

    i2cReceive = !(SMB0DAT & 0x01);
    // Nack defaults to 1 so no need to set it

    //Clear receive count if we are about to receive OR
    // if we have no rx phase (we are a write only xfer)
    // We leave rxCount alone if this is a repeated start
    if(i2cReceive | !i2cBusy)
    {
      sRxCount = 0;
    }

    // If this is a read then fire command received
    // either a read or a repeated start has been requested
    if (!i2cReceive) {
      if(sRxCount || !i2cBusy){
        //Issue a command received callback if there is RX data or if this
        // is not a repeated start
        I2C0_commandReceivedCb();
      }

      SMB0DAT = *sTxBuffer++;
      //DO NOT dec sTxSize This is handled AFTER the first byte is transmited
    }

    //tailchain: clear_start
    SMB0CN0_STA = 0;
    i2cBusy = true;
    break;

  // Slave Receiver: Data received
  case I2C0_SLAVE_RXDATA:
    //Read data into buffer
    sRxBuffer[sRxCount++] = SMB0DAT;

    //If buffer is about to overflow callback user and reset buffer
    if (sRxCount == sRxSize)
    {
      I2C0_commandReceivedCb();
      sRxCount = 0;
    }

    //ACK = 1 by default so no need to set it
    break;

  // Slave Receiver: Stop received while either a Slave Receiver or Transmitter
  case I2C0_SLAVE_RX_STOP:
    if (i2cReceive && sRxCount)
    {
      I2C0_commandReceivedCb();
    }

    //clear stop
    SMB0CN0_STO = 0;

    //If master request pending send a start.
    //tailchain: stop_seq
    SMB0CN0_STA = i2cReq;
    i2cReq = false;
    i2cBusy = false;
    break;

  default:
    // All other cases are an error
    I2C0_errorCb(I2C0_UNKNOWN_ERROR);
    // ----------------------------------------------------------------------
  } // end switch

  //Clear interrupt flag
  SMB0CN0_SI = 0;

}

#endif //EFM8PDL_I2C0_USE_BUFFER
