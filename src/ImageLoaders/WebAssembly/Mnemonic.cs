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

namespace Reko.ImageLoaders.WebAssembly
{
    public enum Mnemonic
    {
        unreachable = 0x00,
        nop = 0x01,
        block = 0x02,
        loop = 0x03,
        @if = 0x04,
        @else = 0x05,
        end = 0x0b,
        br = 0x0c,
        br_if = 0x0d,
        br_table = 0x0e,
        @return = 0x0f,

        call = 0x10,
        call_indirect = 0x11,

        drop = 0x1a,
        select = 0x1b,
        try_table = 0x1F,
        get_local = 0x20,
        set_local = 0x21,
        tee_local = 0x22,
        get_global = 0x23,
        set_global = 0x24,
        table_get = 0x25,
        table_set = 0x26,
        i32_load = 0x28,
        i64_load = 0x29,
        f32_load = 0x2a,
        f64_load = 0x2b,
        i32_load8_s = 0x2c,
        i32_load8_u = 0x2d,
        i32_load16_s = 0x2e,
        i32_load16_u = 0x2f,
        i64_load8_s = 0x30,
        i64_load8_u = 0x31,
        i64_load16_s = 0x32,
        i64_load16_u = 0x33,
        i64_load32_s = 0x34,
        i64_load32_u = 0x35,
        i32_store = 0x36,
        i64_store = 0x37,
        f32_store = 0x38,
        f64_store = 0x39,
        i32_store8 = 0x3a,
        i32_store16 = 0x3b,
        i64_store8 = 0x3c,
        i64_store16 = 0x3d,
        i64_store32 = 0x3e,
        current_memory = 0x3f,
        grow_memory = 0x40,
        i32_const = 0x41,
        i64_const = 0x42,
        f32_const = 0x43,
        f64_const = 0x44,
        i32_eqz = 0x45,
        i32_eq = 0x46,
        i32_ne = 0x47,
        i32_lt_s = 0x48,
        i32_lt_u = 0x49,
        i32_gt_s = 0x4a,
        i32_gt_u = 0x4b,
        i32_le_s = 0x4c,
        i32_le_u = 0x4d,
        i32_ge_s = 0x4e,
        i32_ge_u = 0x4f,
        i64_eqz = 0x50,
        i64_eq = 0x51,
        i64_ne = 0x52,
        i64_lt_s = 0x53,
        i64_lt_u = 0x54,
        i64_gt_s = 0x55,
        i64_gt_u = 0x56,
        i64_le_s = 0x57,
        i64_le_u = 0x58,
        i64_ge_s = 0x59,
        i64_ge_u = 0x5a,
        f32_eq = 0x5b,
        f32_ne = 0x5c,
        f32_lt = 0x5d,
        f32_gt = 0x5e,
        f32_le = 0x5f,
        f32_ge = 0x60,
        f64_eq = 0x61,
        f64_ne = 0x62,
        f64_lt = 0x63,
        f64_gt = 0x64,
        f64_le = 0x65,
        f64_ge = 0x66,

        i32_clz = 0x67,
        i32_ctz = 0x68,
        i32_popcnt = 0x69,
        i32_add = 0x6a,
        i32_sub = 0x6b,
        i32_mul = 0x6c,
        i32_div_s = 0x6d,
        i32_div_u = 0x6e,
        i32_rem_s = 0x6f,
        i32_rem_u = 0x70,
        i32_and = 0x71,
        i32_or = 0x72,
        i32_xor = 0x73,
        i32_shl = 0x74,
        i32_shr_s = 0x75,
        i32_shr_u = 0x76,
        i32_rotl = 0x77,
        i32_rotr = 0x78,
        i64_clz = 0x79,
        i64_ctz = 0x7a,
        i64_popcnt = 0x7b,
        i64_add = 0x7c,
        i64_sub = 0x7d,
        i64_mul = 0x7e,
        i64_div_s = 0x7f,
        i64_div_u = 0x80,
        i64_rem_s = 0x81,
        i64_rem_u = 0x82,
        i64_and = 0x83,
        i64_or = 0x84,
        i64_xor = 0x85,
        i64_shl = 0x86,
        i64_shr_s = 0x87,
        i64_shr_u = 0x88,
        i64_rotl = 0x89,
        i64_rotr = 0x8a,
        f32_abs = 0x8b,
        f32_neg = 0x8c,
        f32_ceil = 0x8d,
        f32_floor = 0x8e,
        f32_trunc = 0x8f,
        f32_nearest = 0x90,
        f32_sqrt = 0x91,
        f32_add = 0x92,
        f32_sub = 0x93,
        f32_mul = 0x94,
        f32_div = 0x95,
        f32_min = 0x96,
        f32_max = 0x97,
        f32_copysign = 0x98,
        f64_abs = 0x99,
        f64_neg = 0x9a,
        f64_ceil = 0x9b,
        f64_floor = 0x9c,
        f64_trunc = 0x9d,
        f64_nearest = 0x9e,
        f64_sqrt = 0x9f,
        f64_add = 0xa0,
        f64_sub = 0xa1,
        f64_mul = 0xa2,
        f64_div = 0xa3,
        f64_min = 0xa4,
        f64_max = 0xa5,
        f64_copysign = 0xa6,

        i32_wrap_i64 = 0xa7,
        i32_trunc_s_f32 = 0xa8,
        i32_trunc_u_f32 = 0xa9,
        i32_trunc_s_f64 = 0xaa,
        i32_trunc_u_f64 = 0xab,
        i64_extend_s_i32 = 0xac,
        i64_extend_u_i32 = 0xad,
        i64_trunc_s_f32 = 0xae,
        i64_trunc_u_f32 = 0xaf,
        i64_trunc_s_f64 = 0xb0,
        i64_trunc_u_f64 = 0xb1,
        f32_convert_s_i32 = 0xb2,
        f32_convert_u_i32 = 0xb3,
        f32_convert_s_i64 = 0xb4,
        f32_convert_u_i64 = 0xb5,
        f32_demote_f64 = 0xb6,
        f64_convert_s_i32 = 0xb7,
        f64_convert_u_i32 = 0xb8,
        f64_convert_s_i64 = 0xb9,
        f64_convert_u_i64 = 0xba,
        f64_promote_f32 = 0xbb,

        i32_reinterpret_f32 = 0xbc,
        i64_reinterpret_f64 = 0xbd,
        f32_reinterpret_i32 = 0xbe,
        f64_reinterpret_i64 = 0xbf,

        i32_extend8_s = 0xC0,
        i32_extend16_s = 0xC1,
        i64_extend8_s = 0xC2,
        i64_extend16_s = 0xC3,
        i64_extend32_s = 0xC4,

        ref_null = 0xD0,
        ref_is_null = 0xD1,
        ref_func = 0xD2,
        ref_eq = 0xD3,

        ref_as_non_null = 0xD4,
        br_on_null = 0xD5,
        br_on_non_null = 0xD6,

        struct_new = 0xFB00,
        struct_new_default = 0xFB01,
        struct_get = 0xFB02,
        struct_get_s = 0xFB03,
        struct_get_u = 0xFB04,
        struct_set = 0xFB05,
        array_new = 0xFB06,
        array_new_default = 0xFB07,
        array_new_fixed = 0xFB08,
        array_new_data = 0xFB09,
        array_new_elem = 0xFB0A,
        array_get = 0xFB0B,
        array_get_s = 0xFB0C,
        array_get_u = 0xFB0D,
        array_set = 0xFB0E,
        array_len = 0xFB0F,
        array_fill = 0xFB10,
        array_copy = 0xFB11,
        array_init_data = 0xFB12,
        array_init_elem = 0xFB13,
        ref_test = 0xFB14,
        ref_test_null = 0xFB15,
        ref_cast = 0xFB16,
        ref_cast_null = 0xFB17,
        br_on_cast = 0xFB18,
        br_on_cast_fail = 0xFB18,
        any_convert_extern = 0xFB1A,
        extern_convert_any = 0xFB1B,
        ref_i31 = 0xFB1C,
        i31_get_s = 0xFB1D,
        i31_get_u = 0xFB1E,

        i32_trunc_sat_f32_s = 0xFC00,
        i32_trunc_sat_f32_u = 0xFC01,
        i32_trunc_sat_f64_s = 0xFC02,
        i32_trunc_sat_f64_u = 0xFC03,
        i64_trunc_sat_f32_s = 0xFC04,
        i64_trunc_sat_f32_u = 0xFC05,
        i64_trunc_sat_f64_s = 0xFC06,
        i64_trunc_sat_f64_u = 0xFC07,

        memory_init = 0xFC08,
        data_drop = 0xFC09,
        memory_copy = 0xFC0A,
        memory_fill = 0xFC0B,

        table_init = 0xFC0C,
        elem_drop = 0xFC0D,
        table_copy = 0xFC0E,
        table_grow = 0xFC0F,
        table_size = 0xFC10,
        table_fill = 0xFC11,

        v128_load = 0xFD_000,
        v128_load8x8_s = 0xFD_001,
        v128_load8x8_u = 0xFD_002,
        v128_load16x4_s = 0xFD_003,
        v128_load16x4_u = 0xFD_004,
        v128_load32x2_s = 0xFD_005,
        v128_load32x2_u = 0xFD_006,
        v128_load8_splat = 0xFD_007,
        v128_load16_splat = 0xFD_008,
        v128_load32_splat = 0xFD_009,
        v128_load64_splat = 0xFD_00A,
        v128_store = 0xFD_00B,
        v128_const = 0xFD_00C,

        i8x16_shuffle = 0xFD_00D,
        i8x16_swizzle = 0xFD_00E,
        i8x16_splat = 0xFD_00F,

        i16x8_splat = 0xFD_010,
        i32x4_splat = 0xFD_011,
        i64x2_splat = 0xFD_012,
        f32x4_splat = 0xFD_013,
        f64x2_splat = 0xFD_014,
        i8x16_extract_lane_s = 0xFD_015,
        i8x16_extract_lane_u = 0xFD_016,
        i8x16_replace_lane = 0xFD_017,
        i16x8_extract_lane_s = 0xFD_018,
        i16x8_extract_lane_u = 0xFD_019,
        i16x8_replace_lane = 0xFD_01A,
        i32x4_extract_lane = 0xFD_01B,
        i32x4_replace_lane = 0xFD_01C,
        i64x2_extract_lane = 0xFD_01D,
        i64x2_replace_lane = 0xFD_01E,
        f32x4_extract_lane = 0xFD_01F,

        f32x4_replace_lane = 0xFD_020,
        f64x2_extract_lane = 0xFD_021,
        f64x2_replace_lane = 0xFD_022,
        i8x16_eq = 0xFD_023,    //35
        i8x16_ne = 0xFD_024,    //36
        i8x16_lt_s = 0xFD_025,    //37
        i8x16_lt_u = 0xFD_026,    //38
        i8x16_gt_s = 0xFD_027,    //39
        i8x16_gt_u = 0xFD_028,    //40
        i8x16_le_s = 0xFD_029,    //41
        i8x16_le_u = 0xFD_02A,    //42
        i8x16_ge_s = 0xFD_02B,    //43
        i8x16_ge_u = 0xFD_02C,    //44
        i16x8_eq = 0xFD_02D,    //45
        i16x8_ne = 0xFD_02E,    //46
        i16x8_lt_s = 0xFD_02F,    //47

        i16x8_lt_u = 0xFD_030,    //48
        i16x8_gt_s = 0xFD_031,    //49
        i16x8_gt_u = 0xFD_032,    //50
        i16x8_le_s = 0xFD_033,    //51
        i16x8_le_u = 0xFD_034,    //52
        i16x8_ge_s = 0xFD_035,    //53
        i16x8_ge_u = 0xFD_036,    //54
        i32x4_eq = 0xFD_037,    //55
        i32x4_ne = 0xFD_038,    //56
        i32x4_lt_s = 0xFD_039,    //57
        i32x4_lt_u = 0xFD_03A,    //58
        i32x4_gt_s = 0xFD_03B,    //59
        i32x4_gt_u = 0xFD_03C,    //60
        i32x4_le_s = 0xFD_03D,    //61
        i32x4_le_u = 0xFD_03E,    //62
        i32x4_ge_s = 0xFD_03F,    //63

        i32x4_ge_u = 0xFD_040,    //64
        f32x4_eq = 0xFD_041,    //65
        f32x4_ne = 0xFD_042,    //66
        f32x4_lt = 0xFD_043,    //67
        f32x4_gt = 0xFD_044,    //68
        f32x4_le = 0xFD_045,    //69
        f32x4_ge = 0xFD_046,    //70
        f64x2_eq = 0xFD_047,    //71
        f64x2_ne = 0xFD_048,    //72
        f64x2_lt = 0xFD_049,    //73
        f64x2_gt = 0xFD_04A,    //74
        f64x2_le = 0xFD_04B,    //75
        f64x2_ge = 0xFD_04C,    //76
        v128_not = 0xFD_04D,    //77
        v128_and = 0xFD_04E,    //78
        v128_andnot = 0xFD_04F,    //79

        v128_or = 0xFD_050,    //80
        v128_xor = 0xFD_051,    //81
        v128_bitselect = 0xFD_052,    //82
        v128_any_true = 0xFD_053,    //83
        v128_load8_lane = 0xFD_054,
        v128_load16_lane = 0xFD_055,
        v128_load32_lane = 0xFD_056,
        v128_load64_lane = 0xFD_057,
        v128_store8_lane = 0xFD_058,
        v128_store16_lane = 0xFD_059,
        v128_store32_lane = 0xFD_05A,
        v128_store64_lane = 0xFD_05B,
        v128_load32_zero = 0xFD_05C,
        v128_load64_zero = 0xFD_05D,
        f32x4_demote_f64x2_zero = 0xFD_05E,
        f64x2_promote_low_f32x4 = 0xFD_05F,

        i8x16_abs = 0xFD_060,    //96
        i8x16_neg = 0xFD_061,    //97
        i8x16_popcnt = 0xFD_062,    //98
        i8x16_all_true = 0xFD_063,    //99
        i8x16_bitmask = 0xFD_064,    //100
        i8x16_narrow_i16x8_s = 0xFD_065,    //101
        i8x16_narrow_i16x8_u = 0xFD_066,    //102
        f32x4_ceil = 0xFD_067,    //103
        f32x4_floor = 0xFD_068,    //104
        f32x4_trunc = 0xFD_069,    //105
        f32x4_nearest = 0xFD_06A,    //106
        i8x16_shl = 0xFD_06B,    //107
        i8x16_shr_s = 0xFD_06C,    //108
        i8x16_shr_u = 0xFD_06D,    //109
        i8x16_add = 0xFD_06E,    //110
        i8x16_add_sat_s = 0xFD_06F,    //111

        i8x16_add_sat_u = 0xFD_070,    //112
        i8x16_sub = 0xFD_071,    //113
        i8x16_sub_sat_s = 0xFD_072,    //114
        i8x16_sub_sat_u = 0xFD_073,    //115
        f64x2_ceil = 0xFD_074,    //116
        f64x2_floor = 0xFD_075,    //117
        i8x16_min_s = 0xFD_076,    //118
        i8x16_min_u = 0xFD_077,    //119
        i8x16_max_s = 0xFD_078,    //120
        i8x16_max_u = 0xFD_079,    //121
        f64x2_trunc = 0xFD_07A,    //122
        i8x16_avgr_u = 0xFD_07B,    //123
        i16x8_extadd_pairwise_i8x16_s = 0xFD_07C,    //124
        i16x8_extadd_pairwise_i8x16_u = 0xFD_07D,    //125
        i32x4_extadd_pairwise_i16x8_s = 0xFD_07E,    //126
        i32x4_extadd_pairwise_i16x8_u = 0xFD_07F,    //127

        i16x8_abs = 0xFD_080,    //128
        i16x8_neg = 0xFD_081,    //129
        i16x8_q15mulr_sat_s = 0xFD_082,    //130
        i16x8_all_true = 0xFD_083,    //131
        i16x8_bitmask = 0xFD_084,    //132
        i16x8_narrow_i32x4_s = 0xFD_085,    //133
        i16x8_narrow_i32x4_u = 0xFD_086,    //134
        i16x8_extend_low_i8x16_s = 0xFD_087,    //135
        i16x8_extend_high_i8x16_s = 0xFD_088,    //136
        i16x8_extend_low_i8x16_u = 0xFD_089,    //137
        i16x8_extend_high_i8x16_u = 0xFD_08A,    //138
        i16x8_shl = 0xFD_08B,    //139
        i16x8_shr_s = 0xFD_08C,    //140
        i16x8_shr_u = 0xFD_08D,    //141
        i16x8_add = 0xFD_08E,    //142
        i16x8_add_sat_s = 0xFD_08F,    //143
        
        i16x8_add_sat_u = 0xFD_090,    //144
        i16x8_sub = 0xFD_091,    //145
        i16x8_sub_sat_s = 0xFD_092,    //146
        i16x8_sub_sat_u = 0xFD_093,    //147
        f64x2_nearest = 0xFD_094,    //148
        i16x8_mul = 0xFD_095,    //149
        i16x8_min_s = 0xFD_096,    //150
        i16x8_min_u = 0xFD_097,    //151
        i16x8_max_s = 0xFD_098,    //152
        i16x8_max_u = 0xFD_099,    //153
        i16x8_avgr_u = 0xFD_09B,    //155
        i16x8_extmul_low_i8x16_s = 0xFD_09C,    //156
        i16x8_extmul_high_i8x16_s = 0xFD_09D,    //157
        i16x8_extmul_low_i8x16_u = 0xFD_09E,    //158
        i16x8_extmul_high_i8x16_u = 0xFD_09F,    //159

        i32x4_abs = 0xFD_0A0,    //160
        i32x4_neg = 0xFD_0A1,    //161
        i32x4_all_true = 0xFD_0A3,    //163
        i32x4_bitmask = 0xFD_0A4,    //164
        i32x4_extend_low_i16x8_s = 0xFD_0A7,    //167
        i32x4_extend_high_i16x8_s = 0xFD_0A8,    //168
        i32x4_extend_low_i16x8_u = 0xFD_0A9,    //169
        i32x4_extend_high_i16x8_u = 0xFD_0AA,    //170
        i32x4_shl = 0xFD_0AB,    //171
        i32x4_shr_s = 0xFD_0AC,    //172
        i32x4_shr_u = 0xFD_0AD,    //173
        i32x4_add = 0xFD_0AE,    //174

        i32x4_sub = 0xFD_0B1,    //177
        i32x4_mul = 0xFD_0B5,    //181
        i32x4_min_s = 0xFD_0B6,    //182
        i32x4_min_u = 0xFD_0B7,    //183
        i32x4_max_s = 0xFD_0B8,    //184
        i32x4_max_u = 0xFD_0B9,    //185
        i32x4_dot_i16x8_s = 0xFD_0BA,    //186
        i32x4_extmul_low_i16x8_s = 0xFD_0BC,    //188
        i32x4_extmul_high_i16x8_s = 0xFD_0BD,    //189
        i32x4_extmul_low_i16x8_u = 0xFD_0BE,    //190
        i32x4_extmul_high_i16x8_u = 0xFD_0BF,    //191
        i64x2_abs = 0xFD_0C0,    //192
        i64x2_neg = 0xFD_0C1,    //193
        i64x2_all_true = 0xFD_0C3,    //195
        i64x2_bitmask = 0xFD_0C4,    //196
        i64x2_extend_low_i32x4_s = 0xFD_0C7,    //199
        i64x2_extend_high_i32x4_s = 0xFD_0C8,    //200
        i64x2_extend_low_i32x4_u = 0xFD_0C9,    //201
        i64x2_extend_high_i32x4_u = 0xFD_0CA,    //202
        i64x2_shl = 0xFD_0CB,    //203
        i64x2_shr_s = 0xFD_0CC,    //204
        i64x2_shr_u = 0xFD_0CD,    //205
        i64x2_add = 0xFD_0CE,    //206

        i64x2_sub = 0xFD_0D1,    //209
        i64x2_mul = 0xFD_0D5,    //213
        i64x2_eq = 0xFD_0D6, // 214
        i64x2_ne = 0xFD_0D7, // 215
        i64x2_lt_s = 0xFD_0D8, // 216
        i64x2_gt_s = 0xFD_0D9, // 217
        i64x2_le_s = 0xFD_0DA, // 218
        i64x2_ge_s = 0xFD_0DB, // 219
        i64x2_extmul_low_i32x4_s = 0xFD_0DC,    //220
        i64x2_extmul_high_i32x4_s = 0xFD_0DD,    //221
        i64x2_extmul_low_i32x4_u = 0xFD_0DE,    //222
        i64x2_extmul_high_i32x4_u = 0xFD_0DF,    //223

        f32x4_abs = 0xFD_0E0,    //224
        f32x4_neg = 0xFD_0E1,    //225
        f32x4_sqrt = 0xFD_0E3,    //227
        f32x4_add = 0xFD_0E4,    //228
        f32x4_sub = 0xFD_0E5,    //229
        f32x4_mul = 0xFD_0E6,    //230
        f32x4_div = 0xFD_0E7,    //231
        f32x4_min = 0xFD_0E8,    //232
        f32x4_max = 0xFD_0E9,    //233
        f32x4_pmin = 0xFD_0EA,    //234
        f32x4_pmax = 0xFD_0EB,    //235
        f64x2_abs = 0xFD_0EC,
        f64x2_neg = 0xFD_0ED,
        f64x2_sqrt = 0xFD_0EF,

        f64x2_add = 0xFD_0F0,
        f64x2_sub = 0xFD_0F1,
        f64x2_mul = 0xFD_0F2,
        f64x2_div = 0xFD_0F3,
        f64x2_min = 0xFD_0F4,
        f64x2_max = 0xFD_0F5,
        f64x2_pmin = 0xFD_0F6,
        f64x2_pmax = 0xFD_0F7,
        i32x4_trunc_sat_f32x4_s = 0xFD_0F8,
        i32x4_trunc_sat_f32x4_u = 0xFD_0F9,
        f32x4_convert_i32x4_s = 0xFD_0FA,
        f32x4_convert_i32x4_u = 0xFD_0FB,
        i32x4_trunc_sat_f64x2_s_zero = 0xFD_0FC,
        i32x4_trunc_sat_f64x2_u_zero = 0xFD_0FD,
        f64x2_convert_low_i32x4_s = 0xFD_0FE,
        f64x2_convert_low_i32x4_u = 0xFD_0FF,

        i16x8_relaxed_swizzle = 0xFD_100,
        i32x4_relaxed_trunc_f32x4_s = 0xFD_101,
        i32x4_relaxed_trunc_f32x4_u = 0xFD_102,
        i32x4_relaxed_trunc_f32x4_s_zero = 0xFD_103,
        i32x4_relaxed_trunc_f32x4_u_zero = 0xFD_104,
        f32x4_relaxed_madd = 0xFD_105,
        f32x4_relaxed_nmadd = 0xFD_106,
        f64x2_relaxed_madd = 0xFD_107,
        f64x2_relaxed_nmadd = 0xFD_108,
        i8x16_relaxed_laneselect = 0xFD_109,
        i16x8_relaxed_laneselect = 0xFD_10A,
        i32x4_relaxed_laneselect = 0xFD_10B,
        i64x2_relaxed_laneselect = 0xFD_10C,
        f32x4_relaxed_min = 0xFD_10D,
        f32x4_relaxed_max = 0xFD_10E,
        f64x2_relaxed_min = 0xFD_10F,

        f64x2_relaxed_max = 0xFD_110,
        i16x8_relaxed_q15mulr_s = 0xFD_111,
        i16x8_relaxed_dot_i8x16_i7x16_s = 0xFD_112,
        i16x8_relaxed_dot_i8x16_i7x16_add_s = 0xFD_113,
    }
}
