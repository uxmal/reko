#region License
/*
 * Copyright (C) 2018-2025 Stefano Moioli <smxdev4@gmail.com>.
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it 
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented;
 *    you must not claim that you wrote the original software.
 *    If you use this software in a product, an acknowledgment
 *    in the product documentation would be appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such,
 *    and must not be misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
#endregion
using Reko.Core.Collections;
using Reko.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.Xex
{
    public enum xex2_section_type : byte
    {
        CODE = 1,
        DATA = 2,
        READONLY_DATA = 3
    }

    [Flags]
    public enum xex2_image_flags : uint
    {
        MANUFACTURING_UTILITY = 0x00000002,
        MANUFACTURING_SUPPORT_TOOLS = 0x00000004,
        XGD2_MEDIA_ONLY = 0x00000008,
        CARDEA_KEY = 0x00000100,
        XEIKA_KEY = 0x00000200,
        USERMODE_TITLE = 0x00000400,
        USERMODE_SYSTEM = 0x00000800,
        ORANGE0 = 0x00001000,
        ORANGE1 = 0x00002000,
        ORANGE2 = 0x00004000,
        IPTV_SIGNUP_APPLICATION = 0x00010000,
        IPTV_TITLE_APPLICATION = 0x00020000,
        KEYVAULT_PRIVILEGES_REQUIRED = 0x04000000,
        ONLINE_ACTIVATION_REQUIRED = 0x08000000,
        PAGE_SIZE_4KB = 0x10000000,  // else 64KB
        REGION_FREE = 0x20000000,
        REVOCATION_CHECK_OPTIONAL = 0x40000000,
        REVOCATION_CHECK_REQUIRED = 0x80000000,
    }

    public enum xex2_media_flags : uint
    {
        HARDDISK = 0x00000001,
        DVD_X2 = 0x00000002,
        DVD_CD = 0x00000004,
        DVD_5 = 0x00000008,
        DVD_9 = 0x00000010,
        SYSTEM_FLASH = 0x00000020,
        MEMORY_UNIT = 0x00000080,
        USB_MASS_STORAGE_DEVICE = 0x00000100,
        NETWORK = 0x00000200,
        DIRECT_FROM_MEMORY = 0x00000400,
        RAM_DRIVE = 0x00000800,
        SVOD = 0x00001000,
        INSECURE_PACKAGE = 0x01000000,
        SAVEGAME_PACKAGE = 0x02000000,
        LOCALLY_SIGNED_PACKAGE = 0x04000000,
        LIVE_SIGNED_PACKAGE = 0x08000000,
        XBOX_PACKAGE = 0x10000000,
    }

    [Flags]
    public enum xex2_region_flags : uint
    {
        NTSCU = 0x000000FF,
        NTSCJ = 0x0000FF00,
        NTSCJ_JAPAN = 0x00000100,
        NTSCJ_CHINA = 0x00000200,
        PAL = 0x00FF0000,
        PAL_AU_NZ = 0x00010000,
        OTHER = 0xFF000000,
        ALL = 0xFFFFFFFF,
    }

    public enum xex2_module_flags : uint
    {
        TITLE = 0x00000001,
        EXPORTS_TO_TITLE = 0x00000002,
        SYSTEM_DEBUGGER = 0x00000004,
        DLL_MODULE = 0x00000008,
        MODULE_PATCH = 0x00000010,
        PATCH_FULL = 0x00000020,
        PATCH_DELTA = 0x00000040,
        USER_MODE = 0x00000080,
    }

    public enum xex2_system_flags : uint
    {
        NO_FORCED_REBOOT = 0x00000001,
        FOREGROUND_TASKS = 0x00000002,
        NO_ODD_MAPPING = 0x00000004,
        HANDLE_MCE_INPUT = 0x00000008,
        RESTRICTED_HUD_FEATURES = 0x00000010,
        HANDLE_GAMEPAD_DISCONNECT = 0x00000020,
        INSECURE_SOCKETS = 0x00000040,
        XBOX1_INTEROPERABILITY = 0x00000080,
        DASH_CONTEXT = 0x00000100,
        USES_GAME_VOICE_CHANNEL = 0x00000200,
        PAL50_INCOMPATIBLE = 0x00000400,
        INSECURE_UTILITY_DRIVE = 0x00000800,
        XAM_HOOKS = 0x00001000,
        ACCESS_PII = 0x00002000,
        CROSS_PLATFORM_SYSTEM_LINK = 0x00004000,
        MULTIDISC_SWAP = 0x00008000,
        MULTIDISC_INSECURE_MEDIA = 0x00010000,
        AP25_MEDIA = 0x00020000,
        NO_CONFIRM_EXIT = 0x00040000,
        ALLOW_BACKGROUND_DOWNLOAD = 0x00080000,
        CREATE_PERSISTABLE_RAMDRIVE = 0x00100000,
        INHERIT_PERSISTENT_RAMDRIVE = 0x00200000,
        ALLOW_HUD_VIBRATION = 0x00400000,
        ACCESS_UTILITY_PARTITIONS = 0x00800000,
        IPTV_INPUT_SUPPORTED = 0x01000000,
        PREFER_BIG_BUTTON_INPUT = 0x02000000,
        ALLOW_EXTENDED_SYSTEM_RESERVATION = 0x04000000,
        MULTIDISC_CROSS_TITLE = 0x08000000,
        INSTALL_INCOMPATIBLE = 0x10000000,
        ALLOW_AVATAR_GET_METADATA_BY_XUID = 0x20000000,
        ALLOW_CONTROLLER_SWAPPING = 0x40000000,
        DASH_EXTENSIBILITY_MODULE = 0x80000000,
        // TODO(benvanik): figure out how stored.
        /*ALLOW_NETWORK_READ_CANCEL            = 0x0,
        UNINTERRUPTABLE_READS                = 0x0,
        REQUIRE_FULL_EXPERIENCE              = 0x0,
        GAME_VOICE_REQUIRED_UI               = 0x0,
        CAMERA_ANGLE                         = 0x0,
        SKELETAL_TRACKING_REQUIRED           = 0x0,
        SKELETAL_TRACKING_SUPPORTED          = 0x0,*/
    }

    public enum xex2_header_keys: uint
    {
        RESOURCE_INFO = 0x000002FF,
        FILE_FORMAT_INFO = 0x000003FF,
        DELTA_PATCH_DESCRIPTOR = 0x000005FF,
        BASE_REFERENCE = 0x00000405,
        BOUNDING_PATH = 0x000080FF,
        DEVICE_ID = 0x00008105,
        ORIGINAL_BASE_ADDRESS = 0x00010001,
        ENTRY_POINT = 0x00010100,
        IMAGE_BASE_ADDRESS = 0x00010201,
        IMPORT_LIBRARIES = 0x000103FF,
        CHECKSUM_TIMESTAMP = 0x00018002,
        ENABLED_FOR_CALLCAP = 0x00018102,
        ENABLED_FOR_FASTCAP = 0x00018200,
        ORIGINAL_PE_NAME = 0x000183FF,
        STATIC_LIBRARIES = 0x000200FF,
        TLS_INFO = 0x00020104,
        DEFAULT_STACK_SIZE = 0x00020200,
        DEFAULT_FILESYSTEM_CACHE_SIZE = 0x00020301,
        DEFAULT_HEAP_SIZE = 0x00020401,
        PAGE_HEAP_SIZE_AND_FLAGS = 0x00028002,
        SYSTEM_FLAGS = 0x00030000,
        EXECUTION_INFO = 0x00040006,
        TITLE_WORKSPACE_SIZE = 0x00040201,
        GAME_RATINGS = 0x00040310,
        LAN_KEY = 0x00040404,
        XBOX360_LOGO = 0x000405FF,
        MULTIDISC_MEDIA_IDS = 0x000406FF,
        ALTERNATE_TITLE_IDS = 0x000407FF,
        ADDITIONAL_TITLE_MEMORY = 0x00040801,
        XEX_HEADER_EXPORTS_BY_NAME = 0x00E10402,
    };

    public enum xex2_encryption_type : ushort
    {
        NONE = 0,
        NORMAL = 1
    }

    public enum xex2_compression_type : ushort
    {
        NONE = 0,
        BASIC = 1,
        NORMAL = 2,
        DELTA = 3,
    }

    public class xex2_file_basic_compression_block
    {
        public uint data_size;
        public uint zero_size;

        public xex2_file_basic_compression_block(SpanStream r)
        {
            data_size = r.ReadUInt32();
            zero_size = r.ReadUInt32();
        }

        public xex2_file_basic_compression_block(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_file_basic_compression_info
    {
        private readonly SpanStream r;

        private IEnumerable<xex2_file_basic_compression_block>? _blocks = null;

        public xex2_file_basic_compression_info(SpanStream r)
        {
            this.r = r;
        }

        public IEnumerable<xex2_file_basic_compression_block> blocks(int num)
        {
            if (_blocks is not null) return _blocks;
            

            var cursor = r.SliceHere();
            var ienum = Enumerable.Range(0, num)
                .Select(i => new xex2_file_basic_compression_block(cursor));

            _blocks = new CachedEnumerable<xex2_file_basic_compression_block>(ienum);
            return _blocks;
        }
    }

    public class xex2_compressed_block_info
    {
        public uint block_size;
        public byte[] block_hash;

        public xex2_compressed_block_info(SpanStream r)
        {
            block_size = r.ReadUInt32();
            block_hash = r.ReadBytes(20);
        }

        public xex2_compressed_block_info(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_file_normal_compression_info
    {
        public uint window_size;
        public xex2_compressed_block_info first_block;

        public xex2_file_normal_compression_info(SpanStream r)
        {
            window_size = r.ReadUInt32();
            first_block = new xex2_compressed_block_info(r.SliceHere());
        }

        public xex2_file_normal_compression_info(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_opt_file_format_info
    {
        public uint info_size;
        public xex2_encryption_type encryption_type;
        public xex2_compression_type compression_type;

        private SpanStream? union;

        public xex2_opt_file_format_info() { }

        public xex2_opt_file_format_info(SpanStream r)
        {
            info_size = r.ReadUInt32();
            encryption_type = r.ReadEnum<xex2_encryption_type>();
            compression_type = r.ReadEnum<xex2_compression_type>();
            union = r.SliceHere();
        }

        public xex2_opt_file_format_info(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }

        public xex2_file_basic_compression_info basic_compression_info()
        {
            return new xex2_file_basic_compression_info(union!.SliceHere());
        }
        public xex2_file_normal_compression_info normal_compression_info()
        {
            return new xex2_file_normal_compression_info(union!.SliceHere());
        }
    }

    public struct xex2_version
    {
        public uint value;
    }

    public class xex2_opt_static_library
    {
        public string name;
        public ushort version_major;
        public ushort version_minor;
        public ushort version_build;
        public byte approval_type;
        public byte apporoval_qfe;

        public xex2_opt_static_library(SpanStream r)
        {
            name = r.ReadString(8);
            version_major = r.ReadUInt16();
            version_minor = r.ReadUInt16();
            version_build = r.ReadUInt16();
            approval_type = r.ReadByte();
            approval_type = r.ReadByte();
        }

        public xex2_opt_static_library(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_opt_static_libraries
    {
        public uint size;
        public xex2_opt_static_library first_library;

        public xex2_opt_static_libraries(SpanStream r)
        {
            size = r.ReadUInt32();
            first_library = new xex2_opt_static_library(r);
        }

        public xex2_opt_static_libraries(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_opt_original_pe_name {
        public uint size;
        public sbyte first_char;
        
        public xex2_opt_original_pe_name(Memory<byte> bytes)
        {
            var r = new SpanStream(bytes, Endianness.BigEndian);
            size = r.ReadUInt32();
            first_char = (sbyte)r.ReadByte();
        }
    }

    public class xex2_opt_data_directory
    {
        public uint offset;
        public uint size;

        public xex2_opt_data_directory(Memory<byte> bytes)
        {
            var r = new SpanStream(bytes, Endianness.BigEndian);
            offset = r.ReadUInt32();
            size = r.ReadUInt32();
        }
    }

    public class xex2_opt_tls_info
    {
        public uint slot_count;
        public uint raw_data_address;
        public uint data_size;
        public uint raw_data_size;

        public xex2_opt_tls_info(Memory<byte> bytes)
        {
            var r = new SpanStream(bytes, Endianness.BigEndian);
            slot_count = r.ReadUInt32();
            raw_data_address = r.ReadUInt32(); 
            data_size = r.ReadUInt32();
            raw_data_size = r.ReadUInt32();
        }
    }

    public class xex2_resource 
    {
        public string name;
        public uint address;
        public uint size;

        public xex2_resource(SpanStream r)
        {
            name = r.ReadString(8);
            address = r.ReadUInt32();
            size = r.ReadUInt32();
        }

        public xex2_resource(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}

    }

    public class xex2_opt_resource_info
    {
        public uint size;
        public xex2_resource first_resource;
        
        public xex2_opt_resource_info(Memory<byte> bytes)
        {
            var r = new SpanStream(bytes, Endianness.BigEndian);
            size = r.ReadUInt32();
            first_resource = new xex2_resource(r);
        }
    }

    [Endian(Endianness.BigEndian)]
    public struct xex2_delta_patch
    {
        public uint old_addr;
        public uint new_addr;
        public ushort uncompressed_len;
        public ushort compressed_len;
        public sbyte patch_data;
    }

    [Endian(Endianness.BigEndian)]
    public struct xex2_opt_delta_patch_descriptor
    {
        public uint size;
        public uint target_version_value;
        public uint source_version_value;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
        public byte[] digest_source;
        [MarshalAs (UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] image_key_source;
        public uint size_of_target_headers;     
        public uint delta_headers_source_offset;
        public uint delta_headers_source_size;  
        public uint delta_headers_target_offset;
        public uint delta_image_source_offset;  
        public uint delta_image_source_size;    
        public uint delta_image_target_offset;  
        public xex2_delta_patch info;           
    }

    public class xex2_opt_execution_info
    {
        public uint media_id;            
        public uint version_value;       
        public uint base_version_value;  
        public uint title_id;            
        public byte platform;            
        public byte executable_table;    
        public byte disc_number;         
        public byte disc_count;          
        public uint savegame_id;

        public xex2_opt_execution_info(SpanStream r) {
            media_id = r.ReadUInt32();
            version_value = r.ReadUInt32();
            base_version_value = r.ReadUInt32();
            title_id = r.ReadUInt32();
            platform = r.ReadByte();
            executable_table = r.ReadByte();
            disc_number = r.ReadByte();
            disc_count = r.ReadByte();
            savegame_id = r.ReadUInt32();
        }
        public xex2_opt_execution_info(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }
    }

    public class xex2_opt_import_libraries
    {
        public class _string_table {
            public uint size;
            public uint count;
            public string[] table;

            public int SIZEOF;

            public _string_table(SpanStream r)
            {
                size = r.ReadUInt32();
                count = r.ReadUInt32();

                table = Enumerable.Range(0, (int)count)
                    .Select(_ => {
                        var str = r.ReadCString();
                        r.AlignStream(sizeof(uint));
                        return str;
                     })
                    .ToArray();

                SIZEOF = 8 + (int)size;
            }

            public _string_table(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
            { }
        }

        public uint size;
        public _string_table string_table;

        public xex2_import_library[] import_libraries;

        public int HEADER_SIZEOF;

        private IEnumerable<xex2_import_library> read_import_libraries(SpanStream r)
        {
            // expected to be placed at HEADER_SIZEOF by caller
            while (r.Position < size)
            {
                var posBefore = r.Position;
                var lib = new xex2_import_library(r);
                var posAfter = r.Position;
                Debug.Assert(posAfter - posBefore == lib.size);
                yield return lib;
            }
        }

        public xex2_opt_import_libraries(SpanStream r)
        {
            size = r.ReadUInt32();
            string_table = new _string_table(r);

            HEADER_SIZEOF = 4 + string_table.SIZEOF;

            import_libraries = read_import_libraries(r).ToArray();
        }

        public xex2_opt_import_libraries(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }




    }

    public enum xex2_thunk_type : ushort
    {
        data,
        function
    }

    public class xex2_thunk_data
    {
        public uint value;

        public xex2_thunk_data(uint value)
        {
            this.value = value;
        }

        public ushort Ordinal => (ushort)(value & 0xFFFF);
        public byte LibIndex => (byte) ((value & 0x00FF0000) >> 16);
        public xex2_thunk_type Type => (xex2_thunk_type)((value & 0xFF000000) >> 24);
    }

    public class xex2_import_library
    {
        public uint size;   
        public byte[] next_import_digest;  
        public uint id;                
        public uint version_value;     
        public uint version_min_value; 
        public ushort name_index;      
        public ushort count;           
        
        public uint[] import_table;

        public xex2_import_library(SpanStream r)
        {
            size = r.ReadUInt32();
            next_import_digest = r.ReadBytes(0x14);
            id = r.ReadUInt32();
            version_value = r.ReadUInt32();
            version_min_value = r.ReadUInt32();
            name_index = r.ReadUInt16();
            count = r.ReadUInt16();

            import_table = Enumerable.Range(0, (count))
                .Select(_ => r.ReadUInt32())
                .ToArray();
        }

        public xex2_import_library(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }
    }

    [Endian(Endianness.BigEndian)]
    public struct xex2_opt_generic_u32
    {
        public uint size;
        public uint values;
    }

    public class xex2_opt_header
    {
        public uint key;
        public uint value;
        public uint offset => value;

        public readonly int SIZEOF;

        public xex2_opt_header(SpanStream r)
        {
            r.Mark();
            key = r.ReadUInt32();
            value = r.ReadUInt32();
            SIZEOF = r.SizeOf();
        }

        public xex2_opt_header(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }
    }

    public class xex2_header
    {
        public byte[] magic;
        public xex2_module_flags module_flags;
        public uint header_size;
        public uint reserved;
        public uint security_offset;
        public uint header_count;
        
        public xex2_security_info security_info;
        public xex2_opt_header[] opt_headers;

        public const int SIZEOF = 24;

        public xex2_header()
        {
            magic = new byte[4];
            security_info = new xex2_security_info();
            opt_headers= Array.Empty<xex2_opt_header>();
        }

        public xex2_header(SpanStream r)
        {
            magic = r.ReadBytes(4);
            module_flags = r.ReadFlagsEnum<xex2_module_flags>();
            header_size = r.ReadUInt32();
            reserved = r.ReadUInt32();
            security_offset = r.ReadUInt32();
            header_count = r.ReadUInt32();

            opt_headers = Enumerable.Range(0, (int) header_count)
                .Select(i => new xex2_opt_header(r))
                .ToArray();

            security_info = r.PerformAt(security_offset, () => new xex2_security_info(r));
        }

        public xex2_header(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        { }
    }

    public class xex2_page_descriptor
    {
        public uint value;
        public byte[] data_digest;

        public record _section(uint value)
        {
            public xex2_section_type info => (xex2_section_type)(value & 0xF);
            public uint page_count => value >> 4;
        }
        public _section section;
        

        public xex2_page_descriptor(SpanStream r)
        {
            value = r.ReadUInt32();
            data_digest = r.ReadBytes(0x14);

            section = new _section(value);
        }

        public xex2_page_descriptor(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}
    }

    public class xex2_security_info
    {
        public uint header_size;            
        public uint image_size;             
        public byte[] rsa_signature;      
        public uint unk_108;              
        public uint image_flags;          
        public uint load_address;         
        public byte[] section_digest;
        public uint import_table_count;       
        public byte[] import_table_digest;           
        public byte[] xgd2_media_id;                  
        public byte[] aes_key;                       

        public uint export_table;            
        public byte[] header_digest;          
        public uint region;                   
        public uint allowed_media_types;      
        public uint page_descriptor_count;

        public xex2_page_descriptor[] page_descriptors;
        
        public xex2_security_info()
        {
            rsa_signature = new byte[0x100];
            section_digest = new byte[0x14];
            import_table_digest = new byte[0x14];
            xgd2_media_id = new byte[0x10];
            aes_key = new byte[0x10];
            header_digest = new byte[0x14];
            page_descriptors = Array.Empty<xex2_page_descriptor>();
        }

        public xex2_security_info(SpanStream r)
        {
            header_size = r.ReadUInt32();
            image_size = r.ReadUInt32();
            rsa_signature = r.ReadBytes(0x100);
            unk_108 = r.ReadUInt32();
            image_flags = r.ReadUInt32();
            load_address = r.ReadUInt32();
            section_digest = r.ReadBytes(0x14);
            import_table_count = r.ReadUInt32();
            import_table_digest = r.ReadBytes(0x14);
            xgd2_media_id = r.ReadBytes(0x10);
            aes_key = r.ReadBytes(0x10);
            export_table = r.ReadUInt32();
            header_digest = r.ReadBytes(0x14);
            region = r.ReadUInt32();
            allowed_media_types = r.ReadUInt32();
            page_descriptor_count = r.ReadUInt32();

            page_descriptors = read_page_descriptors(r).ToArray();
        }

        private IEnumerable<xex2_page_descriptor> read_page_descriptors(SpanStream r)
        {
            for(int i=0; i<page_descriptor_count; i++)
            {
                var descr = new xex2_page_descriptor(r);
                yield return descr;
            }
        }

        public xex2_security_info(Memory<byte> bytes) : this(new SpanStream(bytes, Endianness.BigEndian))
        {}

        private static readonly int _array_offset = (
            Marshal.OffsetOf<xex2_security_info>("page_descriptor_count").ToInt32() + sizeof(uint)
        );

        private uint ntohl(uint v)
        {
            var b = BitConverter.GetBytes(v);
            Array.Reverse(b);
            return BitConverter.ToUInt32(b, 0);
        }

        public int page_descriptor_offset(int base_addr, int i)
        {
            return base_addr + _array_offset + (i * Marshal.SizeOf<xex2_page_descriptor>());
        }
    }

    [Endian(Endianness.BigEndian)]
    public struct xex1_security_info
    {
        public uint header_size;
        public uint image_size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x100)]
        public byte[] rsa_signature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
        public byte[] image_digest;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x14)]
        public byte[] import_table_digest;
        public uint load_address;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] aes_key;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] xgd2_media_id;
        public uint region;
        public uint image_flags;
        public uint export_table;
        public uint allowed_media_types;
        public uint page_descriptor_count;
        public xex2_page_descriptor page_descriptors;
    };

    [Endian(Endianness.BigEndian)]
    public struct xex2_export_table
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] magic;         // 0x0
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] modulenumber;  // 0xC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] version;       // 0x14
        public uint imagebaseaddr;    // 0x20 must be <<16 to be accurate
        public uint count;            // 0x24
        public uint base_value;             // 0x28
        public uint ordOffset;  // 0x2C ordOffset[0] + (imagebaseaddr << 16) = function
    }

    public struct X_IMAGE_EXPORT_DIRECTORY
    {
        public uint Characteristics;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Name;
        public uint Base;
        public uint NumberOfFunctions;
        public uint NumberOfNames;
        public uint AddressOfFunctions;     // RVA from base of image
        public uint AddressOfNames;         // RVA from base of image
        public uint AddressOfNameOrdinals;  // RVA from base of image
    };
}
