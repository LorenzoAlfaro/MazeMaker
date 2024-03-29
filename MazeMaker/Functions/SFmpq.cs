﻿using System;
using System.Runtime.InteropServices;


namespace MazeMaker
{

    public static class SFmpq
    {
        //    '  ShadowFlare MPQ API Library. (c) ShadowFlare Software 2002-2009
        //'  License information for this code is in license.txt and
        //'  included in this file at the end of this comment.

        //'  All functions below are actual functions that are part of this
        //'  library and do not need any additional dll files.  It does not
        //'  even require Storm to be able to decompress or compress files.

        //'  This library emulates the interface of Lmpqapi and Storm MPQ
        //'  functions, so it may be used as a replacement for them in
        //'  MPQ extractors/archivers without even needing to recompile
        //'  the program that uses Lmpqapi or Storm.  It has a few features
        //'  not included in Lmpqapi and Storm, such as extra flags for some
        //'  functions, setting the locale ID of existing files, and adding
        //'  files without having to write them somewhere else first.  Also,
        //'  MPQ handles used by functions prefixed with "SFile" and "Mpq"
        //'  can be used interchangably; all functions use the same type
        //'  of MPQ handles.  You cannot, however, use handles from this
        //'  library with storm or lmpqapi or vice-versa.  Doing so will
        //'  most likely result in a crash.

        //'  Revision History:
        //'  (Release date) 1.08 (ShadowFlare)
        //'  - Fixed a buffer overflow that would occur when reading files
        //'    if neither using a buffer that is large enough to contain the
        //'    entire file nor has a size that is a multiple of 4096
        //'  - Added SFileOpenFileAsArchive which opens an archive that is
        //'    contained within an already open archive
        //'  - Added MpqRenameAndSetFileLocale and MpqDeleteFileWithLocale.
        //'    These have extra parameters that allow you to use them with
        //'    files having language codes other than what was last set
        //'    using SFileSetLocale
        //'  - Fixed a bug that caused (listfile) to get cleared if adding
        //'    files with a locale ID other than 0
        //'  - Added MpqOpenArchiveForUpdateEx which allows creating
        //'    archives with different block sizes
        //'  - SFileListFiles can list the contents of bncache.dat without
        //'    needing an external list

        //'  06/12/2002 1.07 (ShadowFlare)
        //'  - No longer requires Storm.dll to compress or decompress
        //'    Warcraft III files
        //'  - Added SFileListFiles for getting names and information
        //'    about all of the files in an archive
        //'  - Fixed a bug with renaming and deleting files
        //'  - Fixed a bug with adding wave compressed files with
        //'    low compression setting
        //'  - Added a check in MpqOpenArchiveForUpdate for proper
        //'    dwMaximumFilesInArchive values (should be a number that
        //'    is a power of 2).  If it is not a proper value, it will
        //'    be rounded up to the next higher power of 2

        //'  05/09/2002 1.06 (ShadowFlare)
        //'  - Compresses files without Storm.dll!
        //'  - If Warcraft III is installed, this library will be able to
        //'    find Storm.dll on its own. (Storm.dll is needed to
        //'    decompress Warcraft III files)
        //'  - Fixed a bug where an embedded archive and the file that
        //'    contains it would be corrupted if the archive was modified
        //'  - Able to open all .w3m maps now

        //'  29/06/2002 1.05 (ShadowFlare)
        //'  - Supports decompressing files from Warcraft III MPQ archives
        //'    if using Storm.dll from Warcraft III
        //'  - Added MpqAddFileToArchiveEx and MpqAddFileFromBufferEx for
        //'    using extra compression types

        //'  29/05/2002 1.04 (ShadowFlare)
        //'  - Files can be compressed now!
        //'  - Fixed a bug in SFileReadFile when reading data not aligned
        //'    to the block size
        //'  - Optimized some of SFileReadFile's code.It can read files
        //'    faster now
        //'  - SFile functions may now be used to access files not in mpq
        //'    archives as you can with the real storm functions
        //'  - MpqCompactArchive will no longer corrupt files with the
        //'    MODCRYPTKEY flag as long as the file is either compressed,
        //'    listed in "(listfile)", is "(listfile)", or is located in
        //'    the same place in the compacted archive; so it is safe
        //'    enough to use it on almost any archive
        //'  - Added MpqAddWaveFromBuffer
        //'  - Better handling of archives with no files
        //'  - Fixed compression with COMPRESS2 flag

        //'  15/05/2002 1.03 (ShadowFlare)
        //'  - Supports adding files with the compression attribute (does
        //'    not actually compress files).  Now archives created with
        //'    this dll can have files added to them through lmpqapi
        //'    without causing staredit to crash
        //'  - SFileGetBasePath and SFileSetBasePath work more like their
        //'    Storm equivalents now
        //'  - Implemented MpqCompactArchive, but it is not finished yet.
        //'    In its current state, I would recommend against using it
        //'    on archives that contain files with the MODCRYPTKEY flag,
        //'    since it will corrupt any files with that flag
        //'  - Added SFMpqGetVersionString2 which may be used in Visual
        //'    Basic to get the version string

        //'  07/05/2002 1.02 (ShadowFlare)
        //'  - SFileReadFile no longer passes the lpOverlapped parameter it
        //'    receives to ReadFile.  This is what was causing the function
        //'    to fail when used in Visual Basic
        //'  - Added support for more Storm MPQ functions
        //'  - GetLastError may now be used to get information about why a
        //'    function failed

        //'  01/05/2002 1.01 (ShadowFlare)
        //'  - Added ordinals for Storm MPQ functions
        //'  - Fixed MPQ searching functionality of SFileOpenFileEx
        //'  - Added a check for whether a valid handle is given when
        //'    SFileCloseArchive is called
        //'  - Fixed functionality of SFileSetArchivePriority when multiple
        //'    files are open
        //'  - File renaming works for all filenames now
        //'  - SFileReadFile no longer reallocates the buffer for each block
        //'    that is decompressed.  This should make SFileReadFile at least
        //'    a little faster

        //'  30/04/2002 1.00 (ShadowFlare)
        //'  - First version.
        //'  - Compression not yet supported
        //'  - Does not use SetLastError yet, so GetLastError will not return any
        //'    errors that have to do with this library
        //'  - MpqCompactArchive not implemented

        //'  Any comments or suggestions are accepted at blakflare@hotmail.com (ShadowFlare)

        //'  License information:

        //'  Copyright (c) 2002-2009, ShadowFlare <blakflare@hotmail.com>
        //'  All rights reserved.

        //'  Redistribution and use in source and binary forms, with or without
        //'  modification, are permitted provided that the following conditions
        //'  are met:

        //'  1. Redistributions of source code must retain the above copyright
        //'     notice, this list of conditions and the following disclaimer.
        //'  2. Redistributions in binary form must reproduce the above copyright
        //'     notice, this list of conditions and the following disclaimer in the
        //'     documentation and/or other materials provided with the distribution.

        //'  THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS "AS IS" AND
        //'  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
        //'  IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
        //'  ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
        //'  FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
        //'  DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
        //'  OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
        //'  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
        //'  LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
        //'  OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
        //'  SUCH DAMAGE.


        //Type SFMPQVERSION
        //    Major As Integer
        //    Minor As Integer
        //    Revision As Integer
        //    Subrevision As Integer
        //End Type
        public struct SFMPQVERSION
        {
            public static int Major;
            public static int Minor;
            public static int Revision;
            public static int Subrevision;
        }

        //' MpqInitialize does nothing.  It is only provided for
        //' compatibility with MPQ archivers that use lmpqapi.
        //Declare Function MpqInitialize Lib "SFmpq.dll" () As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqInitialize();

        //Declare Function MpqGetVersionString Lib "SFmpq.dll" () As String
        [DllImport("SFmpq.dll")]
        public static extern string MpqGetVersionString();

        //Declare Function MpqGetVersion Lib "SFmpq.dll" () As Single
        [DllImport("SFmpq.dll")]
        public static extern Single MpqGetVersion();

        //Declare Sub SFMpqDestroy Lib "SFmpq.dll" () ' This no longer needs to be called.  It is only provided for compatibility with older versions

        //Declare Sub AboutSFMpq Lib "SFmpq.dll" () ' Displays an about page in a web browser (this has only been tested in Internet Explorer). This is only for the dll version of SFmpq
        [DllImport("SFmpq.dll")]
        public static extern void AboutSFMpq();


        //' SFMpqGetVersionString2's return value is the required length of the buffer plus
        //' the terminating null, so use SFMpqGetVersionString2(ByVal 0&, 0) to get the length.

        //Declare Function SFMpqGetVersionString Lib "SFmpq.dll" () As String
        [DllImport("SFmpq.dll")]
        public static extern string SFMpqGetVersionString();

        //Declare Function SFMpqGetVersionString2 Lib "SFmpq.dll" (ByVal lpBuffer As String, ByVal dwBufferLength As Long) As Long
        [DllImport("SFmpq.dll")]
        public static extern long SFMpqGetVersionString2(string lpBuffer,long dwBufferLength);


        //Declare Function SFMpqGetVersion Lib "SFmpq.dll" () As SFMPQVERSION
        [DllImport("SFmpq.dll")]
        public static extern SFMPQVERSION SFMpqGetVersion();


        //' General error codes
        //Public Const MPQ_ERROR_MPQ_INVALID As Long = &H85200065
        public const long MPQ_ERROR_MPQ_INVALID = 0x85200065;

        //Public Const MPQ_ERROR_FILE_NOT_FOUND As Long = &H85200066
        public const long MPQ_ERROR_FILE_NOT_FOUND = 0x85200066;

        //Public Const MPQ_ERROR_DISK_FULL As Long = &H85200068 'Physical write file to MPQ failed. Not sure of exact meaning
        public const long MPQ_ERROR_DISK_FULL = 0x85200068;

        //Public Const MPQ_ERROR_HASH_TABLE_FULL As Long = &H85200069
        public const long MPQ_ERROR_HASH_TABLE_FULL = 0x85200069;

        //Public Const MPQ_ERROR_ALREADY_EXISTS As Long = &H8520006A
        public const long MPQ_ERROR_ALREADY_EXISTS = 0x8520006A;

        //Public Const MPQ_ERROR_BAD_OPEN_MODE As Long = &H8520006C 'When MOAU_READ_ONLY is used without MOAU_OPEN_EXISTING
        public const long MPQ_ERROR_BAD_OPEN_MODE = 0x8520006C;

        //Public Const MPQ_ERROR_COMPACT_ERROR As Long = &H85300001
        public const long MPQ_ERROR_COMPACT_ERROR = 0x85300001;

        //' MpqOpenArchiveForUpdate flags

        //Public Const MOAU_CREATE_NEW As Long = &H0
        public const int MOAU_CREATE_NEW = 0x0;

        //Public Const MOAU_CREATE_ALWAYS As Long = &H8 'Was wrongly named MOAU_CREATE_NEW
        public const long MOAU_CREATE_ALWAYS = 0x8;

        //Public Const MOAU_OPEN_EXISTING As Long = &H4
        public const int MOAU_OPEN_EXISTING = 0x4;

        //Public Const MOAU_OPEN_ALWAYS As Long = &H20
        public const long MOAU_OPEN_ALWAYS = 0x20;

        //Public Const MOAU_READ_ONLY As Long = &H10 'Must be used with MOAU_OPEN_EXISTING
        public const long MOAU_READ_ONLY = 0x10;

        //Public Const MOAU_MAINTAIN_ATTRIBUTES As Long = &H2 'Will be used in a future version to create the (attributes) file
        public const long MOAU_MAINTAIN_ATTRIBUTES = 0x2;

        //Public Const MOAU_MAINTAIN_LISTFILE As Long = &H1
        public const int MOAU_MAINTAIN_LISTFILE = 0x1;

        //' MpqOpenArchiveForUpdateEx constants
        //Public Const DEFAULT_BLOCK_SIZE As Long = 3 ' 512 << number = block size
        public const long DEFAULT_BLOCK_SIZE = 3; //??????

        //Public Const USE_DEFAULT_BLOCK_SIZE As Long = &HFFFF ' Use default block size that is defined internally
        public const long USE_DEFAULT_BLOCK_SIZE = 0xFFFF;

        //' MpqAddFileToArchive flags
        //Public Const MAFA_EXISTS As Long = &H80000000 'Will be added if not present
        public const long MAFA_EXISTS = 0x80000000;

        //Public Const MAFA_UNKNOWN40000000 As Long = &H40000000
        public const long MAFA_UNKNOWN40000000 = 0x40000000;

        //Public Const MAFA_MODCRYPTKEY As Long = &H20000
        public const long MAFA_MODCRYPTKEY = 0x20000;

        //Public Const MAFA_ENCRYPT As Long = &H10000
        public const long MAFA_ENCRYPT = 0x10000;

        //Public Const MAFA_COMPRESS As Long = &H200
        public const int MAFA_COMPRESS = 0x200;

        //Public Const MAFA_COMPRESS2 As Long = &H100
        public const long MAFA_COMPRESS2 = 0x100;

        //Public Const MAFA_REPLACE_EXISTING As Long = &H1
        public const int MAFA_REPLACE_EXISTING = 0x1;

        //' MpqAddFileToArchiveEx compression flags
        //Public Const MAFA_COMPRESS_STANDARD As Long = &H8 'Standard PKWare DCL compression
        public const long MAFA_COMPRESS_STANDARD = 0x8;

        //Public Const MAFA_COMPRESS_DEFLATE As Long = &H2 'ZLib's deflate compression
        public const long MAFA_COMPRESS_DEFLATE = 0x2;

        //Public Const MAFA_COMPRESS_BZIP2   As Long = &H10 'bzip2 compression
        public const long MAFA_COMPRESS_BZIP2 = 0x10;

        //Public Const MAFA_COMPRESS_WAVE As Long = &H81 'Stereo wave compression
        public const long MAFA_COMPRESS_WAVE = 0x81;

        //Public Const MAFA_COMPRESS_WAVE2 As Long = &H41 'Mono wave compression
        public const long MAFA_COMPRESS_WAVE2 = 0x41;

        //' Flags for individual compression types used for wave compression
        //Public Const MAFA_COMPRESS_WAVECOMP1 As Long = &H80 'Main compressor for stereo wave compression
        public const long MAFA_COMPRESS_WAVECOMP1 = 0x80;

        //Public Const MAFA_COMPRESS_WAVECOMP2 As Long = &H40 'Main compressor for mono wave compression
        public const long MAFA_COMPRESS_WAVECOMP2 = 0x40;

        //Public Const MAFA_COMPRESS_WAVECOMP3 As Long = &H1 'Secondary compressor for wave compression
        public const long MAFA_COMPRESS_WAVECOMP3 = 0x1;

        //' ZLib deflate compression level constants (used with MpqAddFileToArchiveEx and MpqAddFileFromBufferEx)
        //Public Const Z_NO_COMPRESSION As Long = 0
        public const long Z_NO_COMPRESSION = 0;

        //Public Const Z_BEST_SPEED As Long = 1
        public const long Z_BEST_SPEED = 1;

        //Public Const Z_BEST_COMPRESSION As Long = 9
        public const long Z_BEST_COMPRESSION = 9;


        //Public Const Z_DEFAULT_COMPRESSION As Long = (-1)
        public const long Z_DEFAULT_COMPRESSION = -1;

        //' MpqAddWAVToArchive quality flags
        //Public Const MAWA_QUALITY_HIGH As Long = 1
        public const long MAWA_QUALITY_HIGH = 1;

        //Public Const MAWA_QUALITY_MEDIUM As Long = 0
        public const long MAWA_QUALITY_MEDIUM = 0;

        //Public Const MAWA_QUALITY_LOW As Long = 2
        public const long MAWA_QUALITY_LOW = 2;

        //' SFileGetFileInfo flags
        //Public Const SFILE_INFO_BLOCK_SIZE As Long = &H1 'Block size in MPQ
        public const long SFILE_INFO_BLOCK_SIZE = 0x1;

        //Public Const SFILE_INFO_HASH_TABLE_SIZE As Long = &H2 'Hash table size in MPQ
        public const long SFILE_INFO_HASH_TABLE_SIZE = 0x2;
        //Public Const SFILE_INFO_NUM_FILES As Long = &H3 'Number of files in MPQ
        public const long SFILE_INFO_NUM_FILES = 0x3;
        //Public Const SFILE_INFO_TYPE As Long = &H4 'Is Long a file or an MPQ?
        public const long SFILE_INFO_TYPE = 0x4;
        //Public Const SFILE_INFO_SIZE As Long = &H5 'Size of MPQ or uncompressed file
        public const long SFILE_INFO_SIZE = 0x5;
        //Public Const SFILE_INFO_COMPRESSED_SIZE As Long = &H6 'Size of compressed file
        public const long SFILE_INFO_COMPRESSED_SIZE = 0x6;
        //Public Const SFILE_INFO_FLAGS As Long = &H7 'File flags (compressed, etc.), file attributes if a file not in an archive
        public const long SFILE_INFO_FLAGS = 0x7;
        //Public Const SFILE_INFO_PARENT As Long = &H8 'Handle of MPQ that file is in
        public const long SFILE_INFO_PARENT = 0x8;
        //Public Const SFILE_INFO_POSITION As Long = &H9 'Position of file pointer in files
        public const long SFILE_INFO_POSITION = 0x9;
        //Public Const SFILE_INFO_LOCALEID As Long = &HA 'Locale ID of file in MPQ
        public const long SFILE_INFO_LOCALEID = 0xA;
        //Public Const SFILE_INFO_PRIORITY As Long = &HB 'Priority of open MPQ
        public const long SFILE_INFO_PRIORITY = 0xB;
        //Public Const SFILE_INFO_HASH_INDEX As Long = &HC 'Hash index of file in MPQ
        public const long SFILE_INFO_HASH_INDEX = 0xC;
        //Public Const SFILE_INFO_BLOCK_INDEX As Long = &HD 'Block table index of file in MPQ
        public const long SFILE_INFO_BLOCK_INDEX = 0xD;

        //' Return values of SFileGetFileInfo when SFILE_INFO_TYPE flag is used
        //Public Const SFILE_TYPE_MPQ As Long = &H1
        public const long SFILE_TYPE_MPQ = 0x1;
        //Public Const SFILE_TYPE_FILE As Long = &H2
        public const long SFILE_TYPE_FILE = 0x2;

        //' SFileListFiles flags
        //Public Const SFILE_LIST_MEMORY_LIST As Long = &H1 ' Specifies that lpFilelists is a file list from memory, rather than being a list of file lists
        public const long SFILE_LIST_MEMORY_LIST = 0x1;
        //Public Const SFILE_LIST_ONLY_KNOWN As Long = &H2 ' Only list files that the function finds a name for
        public const long SFILE_LIST_ONLY_KNOWN = 0x2;
        //Public Const SFILE_LIST_ONLY_UNKNOWN As Long = &H4 ' Only list files that the function does not find a name for
        public const long SFILE_LIST_ONLY_UNKNOWN = 0x4;
        //Public Const SFILE_LIST_FLAG_UNKNOWN As Long = &H8 ' Use without SFILE_LIST_ONLY_KNOWN or SFILE_LIST_FLAG_UNKNOWN to list all files, but will set dwFileExists to 3 if file's name is not found
        public const long SFILE_LIST_FLAG_UNKNOWN = 0x8;

        //Public Const INVALID_HANDLE_VALUE As Long = -1
        public const long INVALID_HANDLE_VALUE = -1;

        //Public Const FILE_BEGIN As Long = 0
        public const long FILE_BEGIN = 0;
        //Public Const FILE_CURRENT As Long = 1
        public const long FILE_CURRENT = 1;
        //Public Const FILE_END As Long = 2
        public const long FILE_END = 2;

        //' SFileOpenArchive flags
        //Public Const SFILE_OPEN_HARD_DISK_FILE As Long = &H0 'Open archive without regard to the drive type it resides on
        public const long SFILE_OPEN_HARD_DISK_FILE = 0x0;
        //Public Const SFILE_OPEN_CD_ROM_FILE As Long = &H1 'Open the archive only if it is on a CD-ROM
        public const long SFILE_OPEN_CD_ROM_FILE = 0x1;
        //Public Const SFILE_OPEN_ALLOW_WRITE As Long = &H8000 'Open file with write access
        public const long SFILE_OPEN_ALLOW_WRITE = 0x8000;

        //' SFileOpenFileEx search scopes
        //Public Const SFILE_SEARCH_CURRENT_ONLY As Long = &H0 'Used with SFileOpenFileEx; only the archive with the handle specified will be searched for the file
        public const long SFILE_SEARCH_CURRENT_ONLY = 0x0;

        //Public Const SFILE_SEARCH_ALL_OPEN As Long = &H1 'SFileOpenFileEx will look through all open archives for the file
        public const long SFILE_SEARCH_ALL_OPEN = 0x1;

        //Type FILELISTENTRY
        //    dwFileExists As Long ' Nonzero if this entry is used
        //    lcLocale As Long ' Locale ID of file
        //    dwCompressedSize As Long ' Compressed size of file
        //    dwFullSize As Long ' Uncompressed size of file
        //    dwFlags As Long ' Flags for file
        //    szFileName(259) As Byte
        //End Type

        public struct FILELISTENTRY //change to Int , to list files
        {
            public static long dwFileExists;
            public static long lcLocale;
            public static long dwCompressedSize;
            public static long dwFullSize;
            public static long dwFlags;
            public static byte[] szFileName = new byte[259];
        }

        //' Storm functions implemented by this library
        //Declare Function SFileOpenArchive Lib "SFmpq.dll" (ByVal lpFileName As String, ByVal dwPriority As Long, ByVal dwFlags As Long, ByRef hMPQ As Long) As Boolean
        [DllImport("bin/Release/SFmpq.dll", CharSet = CharSet.Unicode)]
        public static extern bool SFileOpenArchive(
            [MarshalAs(UnmanagedType.LPStr)]  string lpFileName, int dwPriority, int dwFlags, ref int hMPQ);

        //Declare Function SFileCloseArchive Lib "SFmpq.dll" (ByVal hMPQ As Long) As Boolean
        [DllImport("bin/Release/SFmpq.dll", CharSet = CharSet.Unicode)]
        public static extern bool SFileCloseArchive(int hMPQ);

        //Declare Function SFileOpenFileAsArchive Lib "SFmpq.dll" (ByVal hSourceMPQ As Long, ByVal lpFileName As String, ByVal dwPriority As Long, ByVal dwFlags As Long, ByRef hMPQ As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileOpenFileAsArchive(long hSourceMPQ,string lpFileName,long dwPriority,long dwFlags, ref int hMPQ);

        //Declare Function SFileGetArchiveName Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpBuffer As String, ByVal dwBufferLength As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileGetArchiveName(int hMPQ, string lpBuffer, long dwBufferLength);//byval hMPQ?

        //Declare Function SFileOpenFile Lib "SFmpq.dll" (ByVal lpFileName As String, ByRef hFile As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileOpenFile(string lpFileName, ref int hFile);

        //Declare Function SFileOpenFileEx Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpFileName As String, ByVal dwSearchScope As Long, ByRef hFile As Long) As Boolean
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern bool SFileOpenFileEx(int hMPQ, [MarshalAs(UnmanagedType.LPStr)] string lpFileName, int dwSearchScope, ref int hFile);

        //Declare Function SFileCloseFile Lib "SFmpq.dll" (ByVal hFile As Long) As Boolean
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern bool SFileCloseFile(int hFile);

        //Declare Function SFileGetFileSize Lib "SFmpq.dll" (ByVal hFile As Long, lpFileSizeHigh As Long) As Long
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern int SFileGetFileSize(int hFile, int lpFileSizeHigh);

        //Declare Function SFileGetFileArchive Lib "SFmpq.dll" (ByVal hFile As Long, ByRef hMPQ As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileGetFileArchive(int hFile, ref int hMPQ);

        //Declare Function SFileGetFileName Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpBuffer As String, ByVal dwBufferLength As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileGetFileName(int hMPQ, string lpBuffer, long dwBufferLength);

        //Declare Function SFileSetFilePointer Lib "SFmpq.dll" (ByVal hFile As Long, ByVal lDistanceToMove As Long, lplDistanceToMoveHigh As Long, ByVal dwMoveMethod As Long) As Long
        [DllImport("SFmpq.dll")]
        public static extern long SFileSetFilePointer(int hFile, long lDistanceToMove, long lplDistanceToMoveHigh, long dwMoveMethod);
        //Declare Function SFileReadFile Lib "SFmpq.dll" (ByVal hFile As Long, ByRef lpBuffer As Any, ByVal nNumberOfBytesToRead As Long, lpNumberOfBytesRead As Long, ByRef lpOverlapped As Any) As Boolean
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern bool SFileReadFile(int hFile, byte[] lpBuffer,int nNumberOfBytesToRead,int lpNumberOfBytesRead, ref dynamic lpOverlapped);

        //Declare Function SFileSetLocale Lib "SFmpq.dll" (ByVal nNewLocale As Long) As Long
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern int SFileSetLocale(int nNewLocale);

        //Declare Function SFileGetBasePath Lib "SFmpq.dll" (ByVal lpBuffer As String, ByVal dwBufferLength As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileGetBasePath(string lpBuffer, long dwBufferLength);

        //Declare Function SFileSetBasePath Lib "SFmpq.dll" (ByVal lpNewBasePath As String) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileSetBasePath(string lpNewBasePath);

        //' Extra storm-related functions
        //Declare Function SFileGetFileInfo Lib "SFmpq.dll" (ByVal hFile As Long, ByVal dwInfoType As Long) As Long
        [DllImport("SFmpq.dll")]
        public static extern bool SFileGetFileInfo(int hFile, long dwInfoType);

        //Declare Function SFileSetArchivePriority Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal dwPriority As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileSetArchivePriority(int hMPQ, long dwPriority);

        //Declare Function SFileFindMpqHeader Lib "SFmpq.dll" (ByVal hFile As Long) As Long
        [DllImport("SFmpq.dll")]
        public static extern bool SFileFindMpqHeader(int hFile);

        //Declare Function SFileListFiles Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpFileLists As String, ByRef lpListBuffer As FILELISTENTRY, ByVal dwFlags As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool SFileListFiles(int hMPQ, string lpFileLists, FILELISTENTRY lpListBuffer,long dwFlags);

        //' Archive editing functions implemented by this library
        //Declare Function MpqOpenArchiveForUpdate Lib "SFmpq.dll" (ByVal lpFileName As String, ByVal dwFlags As Long, ByVal dwMaximumFilesInArchive As Long) As Long
        [DllImport("SFmpq.dll")]
        public static extern long MpqOpenArchiveForUpdate(string lpFileName, long dwFlags, long dwMaximumFilesInArchive);

        //Declare Function MpqCloseUpdatedArchive Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal dwUnknown2 As Long) As Long
        [DllImport("bin/Release/SFmpq.dll")]
        public static extern int MpqCloseUpdatedArchive(int hMPQ, int dwUnknown2);

        //Declare Function MpqAddFileToArchive Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpSourceFileName As String, ByVal lpDestFileName As String, ByVal dwFlags As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqAddFileToArchive(int hMPQ, string lpSourceFileName, string lpDestFileName, long dwFlags);

        //Declare Function MpqAddWaveToArchive Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpSourceFileName As String, ByVal lpDestFileName As String, ByVal dwFlags As Long, ByVal dwQuality As Long) As Boolean
        [DllImport("bin/Release/SFmpq.dll", CharSet = CharSet.Unicode)]
        public static extern bool MpqAddWaveToArchive(int hMPQ, [MarshalAs(UnmanagedType.LPStr)] string lpSourceFileName, [MarshalAs(UnmanagedType.LPStr)] string lpDestFileName, int dwFlags, int dwQuality);

        //Declare Function MpqRenameFile Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpcOldFileName As String, ByVal lpcNewFileName As String) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqRenameFile(int hMPQ, string lpcOldFileName, string lpcNewFileName);

        //Declare Function MpqDeleteFile Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpFileName As String) As Boolean
        [DllImport("bin/Release/SFmpq.dll", CharSet = CharSet.Unicode)]
        public static extern bool MpqDeleteFile(int hMPQ, [MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        //Declare Function MpqCompactArchive Lib "SFmpq.dll" (ByVal hMPQ As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqCompactArchive(int hMPQ);


        //' Extra archive editing functions

        //Declare Function MpqOpenArchiveForUpdateEx Lib "SFmpq.dll" (ByVal lpFileName As String, ByVal dwFlags As Long, ByVal dwMaximumFilesInArchive As Long, ByVal dwBlockSize As Long) As Long
        [DllImport("bin/Release/SFmpq.dll", CharSet = CharSet.Unicode)]
        public static extern int MpqOpenArchiveForUpdateEx(
            [MarshalAs(UnmanagedType.LPStr)] string lpFileName, int dwFlags,int dwMaximumFilesInArchive,int dwBlockSize);

        //Declare Function MpqAddFileToArchiveEx Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpSourceFileName As String, ByVal lpDestFileName As String, ByVal dwFlags As Long, ByVal dwCompressionType As Long, ByVal dwCompressLevel As Long) As Boolean
        [DllImport("SFmpq.dll", CharSet = CharSet.Ansi)]
        public static extern bool MpqAddFileToArchiveEx(int hMPQ, string lpSourceFileName, string lpDestFileName, int dwFlags, int dwCompressionType, int dwCompressLevel);

        //Declare Function MpqAddFileFromBufferEx Lib "SFmpq.dll" (ByVal hMPQ As Long, ByRef lpBuffer As Any, ByVal dwLength As Long, ByVal lpFileName As String, ByVal dwFlags As Long, ByVal dwCompressionType As Long, ByVal dwCompressLevel As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqAddFileFromBufferEx(int hMPQ, ref dynamic lpBuffer, long dwLength, string lpFileName, long dwFlags, long dwCompressionType, long dwCompressLevel);

        //Declare Function MpqAddFileFromBuffer Lib "SFmpq.dll" (ByVal hMPQ As Long, ByRef lpBuffer As Any, ByVal dwLength As Long, ByVal lpFileName As String, ByVal dwFlags As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqAddFileFromBuffer(int hMPQ, ref dynamic lpBuffer, long dwLength, string lpFileName, long dwFlags);

        //Declare Function MpqAddWaveFromBuffer Lib "SFmpq.dll" (ByVal hMPQ As Long, ByRef lpBuffer As Any, ByVal dwLength As Long, ByVal lpFileName As String, ByVal dwFlags As Long, ByVal dwQuality As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqAddWaveFromBuffer(int hMPQ, ref dynamic lpBuffer, long dwLength, string lpFileName, long dwFlags, long dwQuality);

        //Declare Function MpqRenameAndSetFileLocale Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpcOldFileName As String, ByVal lpcNewFileName As String, ByVal nOldLocale As Long, ByVal nNewLocale As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqRenameAndSetFileLocale(int hMPQ, string lpcOldFileName, string lpcNewFileName, long nOldLocale, long nNewLocale);

        //Declare Function MpqDeleteFileWithLocale Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpFileName As String, ByVal nLocale As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqDeleteFileWithLocale(int hMPQ, string lpFileName, long nLocale);

        //Declare Function MpqSetFileLocale Lib "SFmpq.dll" (ByVal hMPQ As Long, ByVal lpFileName As String, ByVal nOldLocale As Long, ByVal nNewLocale As Long) As Boolean
        [DllImport("SFmpq.dll")]
        public static extern bool MpqSetFileLocale(int hMPQ, string lpFileName, long nOldLocale, long nNewLocale);

        //' These functions do nothing.  They are only provided for
        //' compatibility with MPQ extractors that use storm.
        //Declare Function SFileDestroy Lib "SFmpq.dll" () As Boolean
        //Declare Sub StormDestroy Lib "SFmpq.dll" ()

        //' Returns 0 if the dll version is equal to the version your program was compiled
        //' with, 1 if the dll is newer, -1 if the dll is older.
        //Function SFMpqCompareVersion() As Long
        //    Dim ExeVersion As SFMPQVERSION, DllVersion As SFMPQVERSION
        //    With ExeVersion
        //        .Major = 1
        //        .Minor = 0
        //        .Revision = 8
        //        .Subrevision = 1
        //    End With
        //    DllVersion = SFMpqGetVersion()
        //    If DllVersion.Major > ExeVersion.Major Then
        //        SFMpqCompareVersion = 1
        //        Exit Function
        //    ElseIf DllVersion.Major<ExeVersion.Major Then
        //        SFMpqCompareVersion = -1
        //        Exit Function
        //    End If
        //    If DllVersion.Minor> ExeVersion.Minor Then
        //        SFMpqCompareVersion = 1
        //        Exit Function
        //    ElseIf DllVersion.Minor<ExeVersion.Minor Then
        //        SFMpqCompareVersion = -1
        //        Exit Function
        //    End If
        //    If DllVersion.Revision> ExeVersion.Revision Then
        //        SFMpqCompareVersion = 1
        //        Exit Function
        //    ElseIf DllVersion.Revision<ExeVersion.Revision Then
        //        SFMpqCompareVersion = -1
        //        Exit Function
        //    End If
        //    If DllVersion.Subrevision> ExeVersion.Subrevision Then
        //        SFMpqCompareVersion = 1
        //        Exit Function
        //    ElseIf DllVersion.Subrevision<ExeVersion.Subrevision Then
        //        SFMpqCompareVersion = -1
        //        Exit Function
        //    End If
        //    SFMpqCompareVersion = 0
        //End Function
    }
}
