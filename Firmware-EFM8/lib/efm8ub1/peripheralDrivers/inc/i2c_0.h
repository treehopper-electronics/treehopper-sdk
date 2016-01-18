/******************************************************************************
 * Copyright (c) 2014 by Silicon Laboratories Inc. All rights reserved.
 *
 * http://developer.silabs.com/legal/version/v11/Silicon_Labs_Software_License_Agreement.txt
 *****************************************************************************/

#ifndef __I2C_0_H__
#define __I2C_0_H__


#include "efm8_config.h"
#include "SI_EFM8UB1_Register_Enums.h"

/**************************************************************************//**
 *
 * @addtogroup i2c_0 I2C0 Driver
 * @{
 *
 * @brief Peripheral driver for I2C0
 *
 * # Introduction #
 *
 * This module contains all the driver content for I2C0
 *
 * ### Memory Usage ###
 *
 * The table below shows the memory consumption of the library with various
 * options. The 'default' entry shows the consumption when most or all available
 * functions are called. Typical consumption is expected to be less than this
 * since there are normally many uncalled functions that will consume no
 * resources.
 *
 * @note It is possible for memory usage to exceed the listed values in rare cases
 *
 * | condition          | CODE | XRAM | IRAM | RAM |
 * |--------------------|------|------|------|-----|
 * |BUFFER              |  584 |   15 |    0 |   0 |
 * |default             |  186 |    0 |    0 |   0 |
 *
 *
 * # Theory of Operation #
 *
 * This driver supports both SMBus and I2C style communication.
 * It can be configured as a high level driver which takes care of the
 * the entire I2C state machine, or it can be used as a lower level library
 * and the state-machine constructed by the user.
 *
 * ### Buffered Operation ###
 *
 * The buffer API is a high level API that is intended to make I2C as simple
 * as possible while sill providing enough flexibility to be useful in most
 * applications.
 *
 * #### Master ####
 *
 * The driver can perform a full read/write transaction and issue a callback
 * when the entire transfer is complete.
 *
 * @image html I2C_readwrite.svg
 *
 * In this example we issue a 1 byte command to the slave device and
 * read 3 bytes of data as a response (repeated start between write and read).
 *
 * ~~~~~.c
 * SI_SEGMENT_VARIABLE(command[4], uint8_t, SI_SEG_XDATA);
 * SI_SEGMENT_VARIABLE(response[32], uint8_t, SI_SEG_XDATA);
 * bool commandComplete;
 *
 * //In some code we need to read from the slave
 * I2C0_init(I2C0_TIMER2_LOW, false);
 * commandComplete = false;
 * command[0] = READ_STATUS; //some code for the slave
 * I2C0_transfer(SLAVE_ADDRESS, command, response, 1, 3);
 * while(!comandComplete);
 * //Do something with status data in command[2:0]
 *
 * //We need to tell our main thread to continue when transfer complete.
 * void I2C0_transferCompleteCb()
 * {
 *   commandComplete();
 * }
 * ~~~~~
 *
 * @image html I2C_example.svg
 *
 * If no receive data is given (rx_len = 0) the library performs a write
 * only operation.
 *
 * @image html I2C_write.svg
 *
 * If no transmit data is provided (tx_len = 0) the library performs a
 * read only operation.
 *
 * @image html I2C_read.svg
 *
 * #### Slave ####
 *
 * For slave operation the library will save incoming data from the master
 * to a buffer and issue a call-back when the master is finished sending.
 * At that point users code can prepare a response for the master to read.
 *
 * A commandReceived callback is issued under the following conditions:
 *
 *  ~ The last byte of the buffer is written to. After the call back returns
 *    the command buffer is reset to empty an more bytes can be received.
 *  ~ A stop is seen and the command buffer contains data.
 *  ~ A repeated start is seen and the command buffer contains data.
 *  ~ A start (not repeated) is seen and the transfer is a read (master reading).
 *    In this case the the command buffer have 0 bytes of data.
 *
 * @note
 * the device will clock stretch while the callback is being executed.
 *
 * In this example we define 3 commands the master can send each command
 * consists of a command number and 2 single byte arguments. We return a
 * single byte response.
 *
 * ~~~~~.c
 * SI_SEGMENT_VARIABLE(command[3], uint8_t, SI_SEG_XDATA);
 * SI_SEGMENT_VARIABLE(response, uint8_t, SI_SEG_XDATA);
 * #define ADD_COMMAND 0x01
 * #define MUL_COMMAND 0x02
 * #define DIV_COMMAND 0x03
 *
 * //In main we must prime the
 * I2C0_init(I2C0_TIMER2_LOW, false);
 * I2C0_initSlave(MY_ADDRESS, command, 3);
 *
 * //This is called whenever the master finishes a write of data.
 * void I2C0_commandReceivedCb()
 * {
 *   if(command[0] == ADD_COMMAND){
 *     response = command[1] + command[2];
 *   }
 *   if(command[0] == ADD_MUL){
 *     response = command[1] * command[2];
 *   }
 *   if(command[0] == ADD_DIV){
 *     response = command[1] / command[2];
 *   }
 *
 *   //Inform driver we have a response
 *   I2C_sendResponse(&response, 1);
 *
 *   //SCL held low during the execution of this function ensuring
 *   //  that data is ready before the master can attempt to read.
 * }
 * ~~~~~
 *
 * ### Manual operation ###
 *
 * If the buffered driver is not suitable for an application functions are
 * provided to manually implement whatever protocol is required. An ISR is
 * provided to do the first level of state decoding but it may be disabled
 * if the user wishes to write their own ISR.
 *
 * The I2C driver will issue callbacks under according to the following
 * state diagrams. In addition to the listed callbacks I2C0_arbLostCb() and
 * I2C0_errorCb() may be issued at any time.
 *
 * @image html I2C_masterwrite_callbacks.svg Master Write
 *
 * @image html I2C_masterread_callbacks.svg Master Read
 *
 * @image html I2C_slaveread_callbacks.svg Slave Write
 *
 * @image html I2C_slavewrite_callbacks.svg Slave Read
 *
 * The driver ISR handles clearing of flags where necessary but the user
 * callback must handle setting of flags such as STA, STO and ACK.
 *
 * ### Hardware Configuration ###
 *
 * This Driver provides a basic level of configuration through the API. However
 * use of the Simplicity Hardware Configuration tool is highly recommended and
 * some options will only be accessible through that tool (or direct register
 * access).
 *
 * By default the slave is disable when I2C0_init is called and enabled when
 * I2C0_initSlave or I2C0_initSlaveAddress are called (depending on which
 * version of the api is being used.) Because of this I2C0_init should alwasy
 * be called first.
 *
 * ~~~~~.c
 * //Init with no slave
 * I2C0_init(I2C0_TIMER2_LOW, false);
 *
 * //Init with slave (buffered
 * I2C0_init(I2C0_TIMER2_LOW, false);
 * I2C0_initSlave(MY_ADDRESS, command, 3);
 *
 * //Init with slave (manual state machine)
 * I2C0_init(I2C0_TIMER2_LOW, false);
 * I2C0_initSlaveAddress(MY_ADDRESS, mask);
 *
 * //This is an error the slave will be disabled by the
 * // second call
 * I2C0_initSlaveAddress(MY_ADDRESS, mask);
 * I2C0_init(I2C0_TIMER2_LOW, false);
 * ~~~~~
 *
 *****************************************************************************/

//Option macro documentation
/**************************************************************************//**
 * @addtogroup i2c_config Driver Configuration
 * @{
 *  
 *  @brief
 *    Driver configuration constants read from efm8_config.h
 * 
 *  This peripheral driver will look for configuration constants in 
 *  **efm8_config.h**. This file is provided/written by the user and should be 
 *  located in a directory that is part of the include path. 
 *  
 ******************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_AUTO_PAGE
 * @brief Controls if the library handles SI_SFR paging.
 *
 * When '1' the I2C0 driver automatically handles setting and restoring
 * SI_SFR page for each call. When disabled the library consumes less code
 * space but the user must ensure that the correct page is set when calling
 * driver functions that access hardware.
 *
 * Default setting is '1' and may be overridden by defining in 'efm8_config.h'.
 *
 *****************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_USE_STATEMACHINE
 * @brief Controls inclusion of I2C0 ISR and callbacks.
 *
 * When '1' the I2C0 ISR in the driver is used and callbacks are
 * made for each I2C state transition.
 *
 * Default setting is '1' if USE is enabled and may be overridden by
 * defining in 'efm8_config.h'.
 *
 *****************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_USE_BUFFER
 * @brief Controls inclusion of tje Buffered I2C transfer API.
 *
 * When '1' an API is provided for high level buffered transfers. This setting
 * is mutually exclusive with EFM8PDL_I2C0_USE_STATEMACHINE
 *
 * Default setting is '1' and may be overridden by defining in 'efm8_config.h'.
 *
 *****************************************************************************/

/**************************************************************************//**
 * @addtogroup i2c_config_buffered Buffered API Options
 * @{
 *****************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_MASTER_RETRIES
 * @brief Controls number of arbitration loss retries.
 *
 * When a master transfer fails due to arbitration loss the library will
 * automatically retry the transfer until it succeeds or EFM8PDL_I2C0_MASTER_RETRIES
 * failures have occurred. If the transfer is not completed successfully
 * the error callback is fired with I2C0_ARBLOST_ERROR as the error.
 *
 * Valid values are between 0 and 255 retries. 255 is the default value
 *
 *****************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_TX_BUFTYPE
 * @brief Controls the type of pointer used when transmitting buffered data.
 *
 * Sets the memory segment for the tx data buffer pointer when EFM8PDL_I2C0_USE_BUFFER is '1'.
 * This applies to both master and slave functions.
 * valid values are:
 *
 * - SI_SEG_XDATA (default)
 * - SI_SEG_PDATA
 * - SI_SEG_IDATA
 * - SI_SEG_CODE
 * - SI_SEG_GENERIC
 *
 * @warning:
 * Use of generic pointers will adversely effect the size and performance
 * of the buffering functions.
 *
 *****************************************************************************/

/**************************************************************************//**
 * @def EFM8PDL_I2C0_RX_BUFTYPE
 * @brief Controls the type of pointer used when receiving buffered data.
 *
 * Sets the memory segment for the rx data buffer pointer when EFM8PDL_UART0_USE_BUFFER
 * is '1'. This applies to both master and slave functions
 * valid values are:
 *
 * - SI_SEG_XDATA (default)
 * - SI_SEG_PDATA
 * - SI_SEG_IDATA
 * - SI_SEG_CODE
 * - SI_SEG_GENERIC
 *
 * @warning:
 * Use of generic pointers will adversely effect the size and performance
 * of the buffering functions.
 *
 *****************************************************************************/

/**  @} (end addtogroup i2c_config_buffered Buffered API Options) */
/**  @} (end addtogroup i2c_config Driver Configuration) */


//Configuration defaults
#ifndef IS_DOXYGEN
  #define IS_DOXYGEN 0
#endif

#ifndef EFM8PDL_I2C0_AUTO_PAGE
 #define EFM8PDL_I2C0_AUTO_PAGE 1
#endif

#ifndef EFM8PDL_I2C0_USE_STATEMACHINE
  #define EFM8PDL_I2C0_USE_STATEMACHINE 0
#endif

#ifndef EFM8PDL_I2C0_USE_BUFFER
  #if (EFM8PDL_I2C0_USE_STATEMACHINE == 1)
    #define EFM8PDL_I2C0_USE_BUFFER 0
  #else
    #define EFM8PDL_I2C0_USE_BUFFER 1
  #endif
#endif

#if ((EFM8PDL_I2C0_USE_STATEMACHINE == 1) && (EFM8PDL_I2C0_USE_BUFFER == 1))
#error("EFM8PDL_I2C0_USE_STATEMACHINE and EFM8PDL_I2C0_USE_BUFFER can not be used at the same time. Disable one.")
#endif

#ifndef EFM8PDL_I2C0_MASTER_RETRIES
  #define EFM8PDL_I2C0_MASTER_RETRIES 255
#endif
#ifndef EFM8PDL_I2C0_TX_BUFTYPE
  #define EFM8PDL_I2C0_TX_BUFTYPE SI_SEG_XDATA
#endif
#ifndef EFM8PDL_I2C0_RX_BUFTYPE
  #define EFM8PDL_I2C0_RX_BUFTYPE SI_SEG_XDATA
#endif

/***************************************************************************//**
 *  @addtogroup i2c0_if Status Flag Enums
 *  @{
 ******************************************************************************/
#define I2C0_MASTER_SF   SMB0CN0_MASTER__BMASK  //!< Master Mode Status Flag
#define I2C0_TXMODE_SF   SMB0CN0_TXMODE__BMASK  //!< TXMode Status Flag
#define I2C0_START_SF    SMB0CN0_STA__BMASK     //!< Start Status Flag
#define I2C0_STOP_SF     SMB0CN0_STO__BMASK     //!< Stop Status Flag
#define I2C0_ACKREQ_SF   SMB0CN0_ACKRQ__BMASK   //!< ACK Request Status Flag
#define I2C0_ARBLOST_SF  SMB0CN0_ARBLOST__BMASK //!< Arbitration Lost Status Flag
#define I2C0_ACK_SF      SMB0CN0_ACK__BMASK     //!< ACK Status Flag
/**  @} (end addtogroup i2c0_if Status Flag Enums) */


// Runtime API
/***************************************************************************//**
 *  @addtogroup i2c0_runtime I2C0 Runtime API
 *  @{
 ******************************************************************************/

/***************************************************************************//**
 * @brief I2C State.
 ******************************************************************************/
typedef enum
{
  I2C0_MASTER_START  = 0xE0,   ///!< start transmitted (master)
  I2C0_MASTER_TXDATA = 0xC0,   ///!< data byte transmitted (master)
  I2C0_MASTER_RXDATA = 0x80,   ///!< data byte received (master)
  I2C0_SLAVE_ADDRESS = 0x20,   ///!< slave address received (slave)
  I2C0_SLAVE_RX_STOP = 0x10,   ///!< STOP detected during write (slave)
  I2C0_SLAVE_RXDATA  = 0x00,   ///!< data byte received (slave)
  I2C0_SLAVE_TXDATA  = 0x40,   ///!< data byte transmitted (slave)
  I2C0_SLAVE_TX_STOP = 0x50,   ///!< STOP detected during a write (slave)
} I2C0_State_t;

/***************************************************************************//**
 * @brief
 * Return the value of the specified interrupt flag
 *
 * @return
 * The state of the interrupt flag.
 *
 ******************************************************************************/
bool I2C0_getIntFlag();

/***************************************************************************//**
 * @brief
 * Clear I2C Interrupt flag.
 *
 ******************************************************************************/
void I2C0_clearIntFlag();

/***************************************************************************//**
 * @brief
 * Return the specified status flag
 *
 * @param flag:
 * Flag to clear. Multiple flags can be cleared by OR-ing the flags.
 *
 * @return
 * Value of the requested status flag/flags.
 *
 *   Valid enums can be found in the Status Flag Enums group.
 ******************************************************************************/
uint8_t I2C0_getStatusFlag(uint8_t flag) reentrant;

/***************************************************************************//**
 * @brief
 * Set the status flag.
 *
 * @param flag:
 * Status bit to change. Multiple flags can be effected by OR-ing the passed flags.
 * @param state:
 * State to set (1 = set). All flags passed in the flag argument will have this value applied.
 *
 * Valid enums can be found in the Status Flag Enums group.
 *
 ******************************************************************************/
void I2C0_setStatusFlag(uint8_t flag, uint8_t state) reentrant;

/***************************************************************************//**
 * @brief
 * Read a byte from I2C.
 *
 * @return
 * Most recent byte received by I2C.
 *
 * If no new byte was received since last call the value of the previous byte will be returned.
 *
 ******************************************************************************/
uint8_t I2C0_read();

/***************************************************************************//**
 * @brief
 * Write a byte to the I2C
 *
 * @param value:
 * Value to write.
 *
 * Write a byte via I2C
 *
 ******************************************************************************/
void I2C0_write(uint8_t value);

/***************************************************************************//**
 * @brief
 * Abort current I2C transfer
 *
 * This is a reset of the I2C state machine that will force the
 * block into listening for the next start. Generally this is done
 * in response to an error event like loss of arbitration.
 *
 ******************************************************************************/
void I2C0_abort();

/***************************************************************************//**
 * @brief
 * Send ACK/NACK
 *
 * @param ack:
 * value of ACK/NACK.
 *
 * Set or clear ACK bit. ACK/NACK will be sent over the bus.
 *
 ******************************************************************************/
void I2C0_ack(bool ack);

//#if EFM8PDL_I2C0_USE_MASTER == 1

/***************************************************************************//**
 * @brief
 * Send a Start.
 *
 * Send a start on the bus (hardware waits for any current transfer to finish)
 *
 ******************************************************************************/
void I2C0_start();

/***************************************************************************//**
 * @brief
 * Send a Stop.
 *
 * Send a stop on the bus.
 *
 ******************************************************************************/
void I2C0_stop();

/**  @} (end addtogroup i2c0_runtime I2C0 Runtime API) */

// Initialization API
/***************************************************************************//**
 * @addtogroup i2c0_init I2C0 Initialization API
 * @{
 ******************************************************************************/

/***************************************************************************//**
 * @brief
 * Clock Selection Enum.
 *
 ******************************************************************************/
typedef enum
{
  I2C0_TIMER0      = SMB0CF_SMBCS__TIMER0,      //!< Select SystemClock/12
  I2C0_TIMER1      = SMB0CF_SMBCS__TIMER1,      //!< Select SystemClock/4
  I2C0_TIMER2_HIGH = SMB0CF_SMBCS__TIMER2_HIGH, //!< Select Timer0 Overflow
  I2C0_TIMER2_LOW  = SMB0CF_SMBCS__TIMER2_LOW,  //!< Select ECI falling edge
} I2C0_Timebase_t;

/***************************************************************************//**
 * @brief
 * Initialize the I2C
 *
 * @param timebase:
 * Timebase selection.
 * @param sclLow:
 * SCL Low timeout enabled if true.
 *
 * This function configures the I2C with the selected options AND :
 * 
 * - SmBus Free Timeout = enabled
 * - Start Detection Window = 0-1 SYSCLK
 * - Hardware Acknowledge = enabled
 * - Slave = disabled.
 *
 * To enable the I2C slave call I2C0_initAddress AFTER calling I2C0_init.
 * Enabling the I2C slave does not prevent the device from operating
 * as a master.
 *
 * Additional configuration can be performed manually, or through Simplicity Hardware Configurator.
 *
 ******************************************************************************/
void I2C0_init(I2C0_Timebase_t  timebase, bool sclLow);

/***************************************************************************//**
 * @brief
 * Initialize the I2C Slave address
 *
 * @param address:
 * slave address
 * @param mask:
 * slave address mask
 *
 * This function configures the I2C slave address an mask. it
 * also enables slave reception (SMB0CF_INH).
 *
 ******************************************************************************/
void I2C0_initSlaveAddress(uint8_t address, uint8_t mask);

/***************************************************************************//**
 * @brief
 * Restore the I2C to it's uninitialized (reset) state.
 *
 * This function restores the SMB block to it's reset state. This
 * includes both master and slave configuration.
 ******************************************************************************/
void I2C0_reset();

/**  @} (end addtogroup i2c0_init I2C0 Initialization API) */

//=========================================================
// StateMachine API
//=========================================================
#if (EFM8PDL_I2C0_USE_STATEMACHINE == 1) || IS_DOXYGEN
/***************************************************************************//**
 * @brief
 * I2C Interrupt handler (not a callback).
 *
 * This ISR is provided by the library when EFM8PDL_I2C0_USE_CALLBACKS = "1".
 *
 ******************************************************************************/
SI_INTERRUPT_PROTO(I2C0_ISR, SMBUS0_IRQn);
#endif //EFM8PDL_I2C0_USE_STATEMACHINE

// Buffered API
/***************************************************************************//**
 *  @addtogroup i2c0_buffered I2C0 Buffered API
 *  @{
 ******************************************************************************/
#if (EFM8PDL_I2C0_USE_BUFFER == 1) || IS_DOXYGEN

/***************************************************************************//**
 * @def void I2C0_ISR()
 * @brief
 * I2C Interrupt handler (not a callback).
 *
 * This ISR is provided by the library when EFM8PDL_I2C0_USE_CALLBACKS = "1".
 *
 ******************************************************************************/

/***************************************************************************//**
 * @brief
 * I2C Transfer Errors.
 *
 ******************************************************************************/
typedef enum
{
  I2C0_ARBLOST_ERROR  = 0x00,   /// Master lost arbitration on EFM8PDL_I2C0_MASTER_RETRIES consecutive attempts
  I2C0_NACK_ERROR     = 0x01,   /// Master got a NACK while writing the slave
  I2C0_UNKNOWN_ERROR  = 0x02,   /// An Unknown error occurred
  I2C0_TXUNDER_ERROR  = 0x03,   /// Master read more data then is available

} I2C0_TransferError_t;


/***************************************************************************//**
 * @brief performs a transfer to the specified address.
 *
 * @param address:
 * Address to write to with LSB (R/W) bit cleared. 6-bit I2C address
 * must be properly placed in bits 7:1 of the address parameter.
 * @param[in] txBuffer:
 * Buffer containing tx data (NULL if unused)
 * @param[out] rxBuffer:
 * Buffer for rx data (NULL if unused)
 * @param tx_len:
 * Number of bytes to transmit from txBuffer
 * @param rx_len:
 * Number of bytes to receive into rxBuffer
 *
 * This function sets up a transfer over I2C to the address defined.
 *
 * When a rx_len of 0 is issued a I2C write will be performed and the application
 * will be called back when the transfer has completed.
 *
 * When tx_len is 0 a I2C read will be performed and the application will be
 * called back when the transfer is complete.
 *
 * When tx_len and rx_len are both non-zero a repeated start will be used.
 * A write of the tx data will be executed followed by a repeated start,
 * Followed by a read of rx data. The application will be called back when
 * the entire transfer is complete.
 *
 * If this function is called while a slave transfer is in progress the
 * requested transfer will take place after the current transfer is completed.
 * This is ONLY valid for slave transfers. If I2C0_transfer is called before the
 * previous master transfer complete it will overwrite the state of the in-progress
 * transfer.
 *
 ******************************************************************************/
void I2C0_transfer(uint8_t address,
                   SI_VARIABLE_SEGMENT_POINTER(txBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE),
                   SI_VARIABLE_SEGMENT_POINTER(rxBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE),
                   uint8_t tx_len,
                   uint8_t rx_len);

/***************************************************************************//**
 * @brief
 * Abort current buffered transfer
 *
 * This function will set rx and tx len to 0 causing the current
 * transfer to end immediately.
 ******************************************************************************/
void I2C0_abortTransfer();

/***************************************************************************//**
 * @brief
 * Get remaining bytes to write.
 *
 * @return
 * Number of bytes to transfer.
 *
 ******************************************************************************/
uint8_t I2C0_txBytesRemaining();

/***************************************************************************//**
 * @brief
 * Get remaining bytes to read.
 *
 * @return
 * Number of bytes to read.
 *
 ******************************************************************************/
uint8_t I2C0_rxBytesRemaining();

/***************************************************************************//**
 * @brief Initializes slave block.
 *
 * @param address:
 * Slaves address only access to this address will be acknowledged
 * @param[out] commandBuffer:
 * Buffer to save incoming data to.
 * @param length:
 * length of command buffer.
 *
 * Any master writes to the slave address will be buffered in commandBuffer.
 * When the end of the write (stop or repeated start) occurs the
 * I2C_commandReceived callback will be fired. The I2C bus will be stalled
 * (SCL held low) until that callback returns.
 *
 * The callback will also be fired if the command buffer becomes full. In all cases
 * the commandBuffer pointer is reset to the head of the buffer when the
 * commandReceived callback returns. The next data received will then be
 * written to the buffer as normal. Because of this the the implementation of the
 * callback should ensure that the buffer is processed before returning.
 *
 ******************************************************************************/
void I2C0_initSlave(uint8_t address,
                   SI_VARIABLE_SEGMENT_POINTER(commandBuffer, uint8_t, EFM8PDL_I2C0_RX_BUFTYPE),
                   uint8_t length);

/***************************************************************************//**
 * @brief
 * Returns length of received command.
 *
 * @return
 * Number of bytes in commandBuffer
 *
 ******************************************************************************/
uint8_t I2C0_getCommandLength();

/***************************************************************************//**
 * @brief Loads return data.
 *
 * @param[out] dataBuffer:
 * Buffer of data to return to the slave.
 * @param length:
 * length of data buffer.
 *
 * This function sets up the data to be returned to the master in the event
 * of an I2C read. It may be called during the commandRecived callback to set up
 * response to a repeated start.
 *
 * There does not exist a mechanism for providing extended (ping-pong) buffers.
 *
 ******************************************************************************/
void I2C0_sendResponse(SI_VARIABLE_SEGMENT_POINTER(dataBuffer, uint8_t, EFM8PDL_I2C0_TX_BUFTYPE),
                      uint8_t length);

#endif //EFM8PDL_I2C0_USE_BUFFER
/**  @} (end addtogroup i2c0_buffered I2C0 Buffered API) */

// Callbacks
/***************************************************************************//**
 *  @addtogroup i2c0_callbacks User Callbacks
 *  @{
 ******************************************************************************/

/***************************************************************************//**
 * @addtogroup i2c0_callbacks_statemachine StateMachine API
 * @{
 *
 * These callbacks will be called by the library when
 * EFM8PDL_I2C0_USE_STATEMACHINE. If the StateMachine api is disabled
 * the callbacks do not need to be provided by the user.
 *
 ******************************************************************************/

#if (EFM8PDL_I2C0_USE_STATEMACHINE == 1) || IS_DOXYGEN
/***************************************************************************//**
 * @brief
 * Callback for arbitration lost.
 *
 * @param state:
 * The state being transitioned to when arbitration was lost
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * arbitration is lost. Details on the state of the transfer during lost of
 * arbitration can be found in the passed state information.
 ******************************************************************************/
extern void I2C0_arbLostCb(I2C0_State_t state);

/***************************************************************************//**
 * @brief
 * Callback for data transmitted.
 *
 * @param master:
 * True if data was transmitted while acting as a master.
 * @param ack:
 * True if transmitted data was ack'd (false if nack).
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * Transmission of a byte is complete.
 *
 ******************************************************************************/
extern void I2C0_TxDataCb(bool master, bool ack);

/***************************************************************************//**
 * @brief
 * Callback for receive of data.
 *
 * @param master:
 * True if device was acting as a master when data received.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * data is received. The implementation should set the ACK status bit to ack the
 * transfer. The bus is stalled (SCL low time extension) until the callback returns
 ******************************************************************************/
extern void I2C0_RxDataCb(bool master);

/***************************************************************************//**
 * @brief
 * Callback for receive of start.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * a start is seen. It is only called when acting as an I2C master. The
 * implementation should write the slave address and R/W bit to the I2C.
 ******************************************************************************/
extern void I2C0_startCb();  //master only

/***************************************************************************//**
 * @brief
 * Callback for address received.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * an address is received. This callback is only fired when acting as a slave. The
 * implementation should ack if the address is for the slave and nack if it is for
 * another slave.
 ******************************************************************************/
extern void I2C0_addressCb();   // slave only

/***************************************************************************//**
 * @brief
 * Callback for stop received.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * a stop is seen. It is only fired when acting as a slave.
 *
 ******************************************************************************/
extern void I2C0_stopCb();      // slave only
#endif //EFM8PDL_I2C0_USE_STATEMACHINE
/**  @} (end addtogroup i2c0_callbacks_statemachine StateMachine API) */


/***************************************************************************//**
 * @addtogroup i2c0_callbacks_buffer Buffer API
 * @{
 *
 * These callbacks will be called by the library when
 * EFM8PDL_I2C0_USE_BUFFER. If the Buffered API is disabled
 * the callbacks do not need to be provided by the user.
 *
 ******************************************************************************/
#if (EFM8PDL_I2C0_USE_BUFFER == 1) || IS_DOXYGEN

/***************************************************************************//**
 * @brief
 * Callback transfer complete.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * a master transmission is completed.
 ******************************************************************************/
extern void I2C0_transferCompleteCb();

/***************************************************************************//**
 * @brief
 * Callback for any error.
 *
 * @param error:
 * I2C transfer error which caused the callback.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 * any error occurs. This includes an unexpected NACK,  arbitration loss, or
 * encountering any unknown or unexpected SMBus/I2C state.
 *
 * In all error conditions the transfer will be aborted.
 *
 ******************************************************************************/
extern void I2C0_errorCb(I2C0_TransferError_t error);

/***************************************************************************//**
 * @brief
 * Callback for slave command reception.
 *
 * @warning
 * This function is called from an ISR and should be as short as possible.
 *
 * This function is defined by the user and called by the peripheral driver when
 *
 * A command is received from the master or the command buffer is full.
 *
 ******************************************************************************/
extern void I2C0_commandReceivedCb();

#endif //EFM8PDL_I2C0_USE_BUFFER
/** @} (end addtogroup i2c0_callbacks_buffer Buffer API) */
/** @} (end addtogroup i2c0_callbacks User Callbacks) */
/** @} (end addtogroup i2c_0 I2C0 Driver) */
#endif //__I2C_0_H__
