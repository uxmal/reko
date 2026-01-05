#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

namespace Reko.Arch.Pdp.Pdp1;

public enum Mnemonic
{
    Invalid,

    add,
    add_i,
    and,
    and_i,
    cal,
    cbs,
    cks,
    cla,
    clf,
    cli,
    cma,
    dac,
    dac_i,
    dap,
    dap_i,
    dio,
    dio_i,
    dip,
    dip_i,
    dis,
    dis_i,
    div,
    div_i,
    dzm,
    dzm_i,
    esm,
    hlt,
    idx,
    idx_i,
    ior,
    ior_i,
    isp,
    isp_i,
    jda,
    jmp,
    jmp_i,
    jsp,
    jsp_i,
    lac,
    lac_i,
    lap,
    lat,
    law,
    lio,
    lio_i,
    lsm,
    mul,
    mul_i,
    mus,
    mus_i,
    nop,
    ppa,
    ppb,
    ral,
    rar,
    rcl,
    rcr,
    ril,
    rir,
    rpa,
    rpb,
    rrb,
    sad,
    sad_i,
    sal,
    sar,
    sas,
    sas_i,
    scl,
    scr,
    sil,
    sir,
    skp,
    skp_i,
    sma,
    sma_i,
    spa,
    spa_i,
    spi,
    spi_i,
    stf,
    sub,
    sub_i,
    sza,
    sza_i,
    szf,
    szf_i,
    szo,
    szo_i,
    szs,
    szs_i,
    tyi,
    tyo,
    xct,
    xct_i,
    xor,
    xor_i,
}