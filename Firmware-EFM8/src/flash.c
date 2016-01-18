/******************************************************************************
 * Copyright (c) 2015 by Silicon Laboratories Inc. All rights reserved.
 *
 * http://developer.silabs.com/legal/version/v11/Silicon_Labs_Software_License_Agreement.txt
 *****************************************************************************/

#include "efm8_usb.h"
#include "flash.h"

// ----------------------------------------------------------------------------
// Writes one byte to flash memory.
// ----------------------------------------------------------------------------
void flash_writeByte(uint16_t addr, uint8_t byte)
{
  uint8_t SI_SEG_XDATA * pwrite = (uint8_t SI_SEG_XDATA *) addr;

  // Unlock flash by writing the key sequence
  FLKEY = 0xA5;
  FLKEY = 0xF1;

  // Enable flash writes, then do the write
  PSCTL |= PSCTL_PSWE__WRITE_ENABLED;
  *pwrite = byte;
  PSCTL &= ~(PSCTL_PSEE__ERASE_ENABLED|PSCTL_PSWE__WRITE_ENABLED);
}

// ----------------------------------------------------------------------------
// Erases one page of flash memory.
// ----------------------------------------------------------------------------
void flash_erasePage(uint16_t addr)
{
  // Enable flash erasing, then start a write cycle on the selected page
  PSCTL |= PSCTL_PSEE__ERASE_ENABLED;
  flash_writeByte(addr, 0);
}

//// ----------------------------------------------------------------------------
//// Writes one byte to flash memory.
//// ----------------------------------------------------------------------------
//void flash_writeByte(uint16_t addr, uint8_t byte)
//{
//  // Don't bother writing the erased value to flash
//  if (byte != 0xFF)
//  {
//    writeByte(addr, byte);
//  }
//}
