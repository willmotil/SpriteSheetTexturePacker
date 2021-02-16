using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Linq;

namespace Microsoft.Xna.Framework
{

    /// <summary>
    /// A self Recursive File Folder class.
    /// Bit old and needs to be cleaned up and probably redone man this is ugly.
    /// </summary>
    public class FoldersAndFiles
    {
        public string directoryFullPath = "";

        public bool HasFiles { get; private set; }
        public bool HasSubfolders { get; private set; }

        public List<string> fullFilePaths = new List<string>();
        public List<string> subFolderFullPaths = new List<string>();

        /// <summary>
        /// no the links are actual refences to the sub folders instances
        /// </summary>
        public List<FoldersAndFiles> children = new List<FoldersAndFiles>();

        /// <summary>
        /// Note the parent link is the actual reference to the instance
        /// </summary>
        public FoldersAndFiles parent;

        public static FoldersAndFiles GetFileFolderHeirarchy(string dirpath)
        {
            return new FoldersAndFiles(dirpath, "Any", null);
        }

        public static FoldersAndFiles GetFileFolderHeirarchy(string dirpath, string file_ext)
        {
            return new FoldersAndFiles(dirpath, file_ext, null);
        }

        public static FoldersAndFiles GetFileFolderHeirarchy(string dirpath, string[] file_exts)
        {
            return new FoldersAndFiles(dirpath, file_exts, null);
        }

        private FoldersAndFiles(string dirpath, string file_ext, FoldersAndFiles parentRef)
        {
            parent = parentRef;
            // add name
            directoryFullPath = dirpath;
            // add files
                AddFiles(MgPathFolderFileOps.GetFileNamesInFolder(dirpath, file_ext));
            string[] childFolders = MgPathFolderFileOps.GetSubFolderNamesOrFullPathsInDirectory(dirpath, true);
            if (childFolders.Length > 0)
            {
                // mark that this folder has subfolders
                HasSubfolders = true;
                // subfolder list
                foreach (string f in childFolders)
                {
                    subFolderFullPaths.Add(f);
                    children.Add(new FoldersAndFiles(f, file_ext, this));
                }
            }
        }

        private FoldersAndFiles(string dirpath,string[] file_exts, FoldersAndFiles parentRef)
        {
            parent = parentRef;
            // add name
            directoryFullPath = dirpath;
            // add files
            foreach(var ext in file_exts)
               AddFiles(MgPathFolderFileOps.GetFilesFullPathNamesInFolder(dirpath, ext));
            string[] childFolders = MgPathFolderFileOps.GetSubFolderNamesOrFullPathsInDirectory(dirpath, true);
            if (childFolders.Length > 0)
            {
                // mark that this folder has subfolders
                HasSubfolders = true;
                // subfolder list
                foreach (string f in childFolders)
                {
                    subFolderFullPaths.Add(f);
                    children.Add(new FoldersAndFiles(f, file_exts, this));
                }
            }
        }

        public void AddFiles(string[] filepaths)
        {
            if (filepaths.Length > 0)
            {
                // mark that this folder has files
                HasFiles = true;
                // add each file name to the folders names list
                foreach (string f in filepaths)
                {
                    fullFilePaths.Add(f);
                }
            }
        }
    }

    /// <summary>
    /// Has methods to help shorten annoying file or folder tasks
    /// </summary>
    public class MgPathFolderFileOps
    {
        public static string ApplicationBasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ApplicationPath = Environment.CurrentDirectory;
        private static string CurrentPath { get; set; }

        public static void ThrowException(string s)
        {
            throw new Exception(s);
        }

        public static FoldersAndFiles GetFileFolderHeirarchy(string dirpath)
        {
            return FoldersAndFiles.GetFileFolderHeirarchy(dirpath);
        }
        public static FoldersAndFiles GetFileFolderHeirarchy(string dirpath, string file_ext)
        {
            return FoldersAndFiles.GetFileFolderHeirarchy(dirpath, file_ext);
        }

        /// <summary>
        /// buncha overloaded versions of combine path
        /// </summary>
        public static string CombinePath(string A, string B)
        {
            string str = Path.Combine(A, B);
            return str;
        }

        public static string CombinePath(string A, string B, string C)
        {
            string str = Path.Combine(A, B); str = Path.Combine(str, C);
            return str;
        }

        public static string CombinePath(string A, string B, string C, string D)
        {
            string str = Path.Combine(A, B); str = Path.Combine(str, C); str = Path.Combine(str, D);
            return str;
        }

        public static string CombinePath(string A, string B, string C, string D, string E)
        {
            string str = Path.Combine(A, B); str = Path.Combine(str, C); str = Path.Combine(str, D); str = Path.Combine(str, E);
            return str;
        }

        public static string CombinePath(string A, string B, string C, string D, string E, string F)
        {
            string str = Path.Combine(A, B); str = Path.Combine(str, C); str = Path.Combine(str, D); str = Path.Combine(str, E); str = Path.Combine(str, F);
            return str;
        }

        public static string CombinePath(params string[] s)
        {
            string result = "";
            for (int i = 0; i < s.Length; i++)
            {
                result = Path.Combine(result, s[i]);
            }
            return result;
        }

        /// <summary>
        /// this uses the String comparer class
        /// searches for value equality 
        /// </summary>
        public static bool CompareString(string t, string a, bool ignorecase)
        {
            bool result = false;
            if (ignorecase)
            {
                if (String.Compare(a, t, true) == 0)
                {
                    result = true;
                }
            }
            else
            {
                if (String.Compare(a, t, false) == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// if a match is found the method returns true
        /// </summary>
        public static bool CompareStringArray(string[] t, string a, bool ignorecase)
        {
            bool result = false;
            for (int i = 0; i < t.Length; i++)
            {
                if (ignorecase)
                {
                    if (String.Compare(a, t[i], true) == 0)
                    {
                        result = true;
                    }
                }
                else
                {
                    if (String.Compare(a, t[i], false) == 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// if a match is found the method returns true and the sIndex will be more then -1
        /// the sIndex returned will be for the first match
        /// </summary>
        public static bool CompareStringArray(string[] t, string a, bool ignorecase, out int sIndex)
        {
            bool result = false;
            sIndex = -1;
            bool firstfound = false;
            for (int i = 0; i < t.Length; i++)
            {
                if (ignorecase)
                {
                    if (String.Compare(a, t[i], true) == 0)
                    {
                        result = true;
                        if (firstfound == false) { sIndex = i; firstfound = true; }
                    }
                }
                else
                {
                    if (String.Compare(a, t[i], false) == 0)
                    {
                        result = true;
                        if (firstfound == false) { sIndex = i; firstfound = true; }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Basically the same as compareStringArray
        /// </summary>
        public static bool ContainsStringArray(string t, string[] a, bool ignorecase)
        {
            bool result = false;
            if (ignorecase)
            {
                result = a.Contains(t, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                result = a.Contains(t);
            }
            return result;
        }


        /// <summary>
        /// simply replace the filename's extention
        /// or a paths extention will be replaced correctly with or without the dot
        /// </summary>
        public static string FileExtensionReplace(string filename_or_fullfilepath, string ext)
        {
            if (filename_or_fullfilepath == null || filename_or_fullfilepath == "" || ext == null || ext == "")
                ThrowException("Invalid path filename or extension");
            return Path.ChangeExtension(filename_or_fullfilepath, ext);
        }

        /// <summary>
        /// replace just the filename the ext on the fullpath remains
        /// </summary>
        public static string FileNameButNotExtReplace(string fullpath, string newfilename)
        {
            string ext = FileExtention(fullpath);
            string name = FileNameFromPathWithoutExtention(fullpath);
            string path = PathRemoveFileName(fullpath);
            string result = Path.Combine(path, newfilename);
            return Path.ChangeExtension(result, ext);
        }

        /// <summary>
        /// replace the filename and the extension seperately in the same method
        /// </summary>
        public static string FileNameAndExtReplace(string fullpath, string newfilename, string ext)
        {
            string path = PathRemoveFileName(fullpath);
            string result = Path.Combine(path, newfilename);
            return Path.ChangeExtension(result, ext);
        }

        /// <summary>
        /// adds a file name and extension to a path without one or with one so add safetys later on
        /// </summary>
        public static string FileNameAndExtAdd(string fullpath, string newfilename, string ext)
        {
            string result = Path.Combine(fullpath, newfilename);
            return Path.ChangeExtension(result, ext);
        }

        /// <summary>
        /// replace the whole filename  and ext at  once
        /// </summary>
        public static string FileNameReplace(string fullpath, string a_proper_filename_with_a_extension)
        {
            string path = PathRemoveFileName(fullpath);
            return Path.Combine(path, a_proper_filename_with_a_extension);
        }

        /// <summary> 
        /// returns true 
        /// if the specified file exists in the directory
        /// </summary>
        /// <returns>true if the file exists and permissions meet the requirements</returns>
        public static bool FileExists(string _fullpath)
        {
            return System.IO.File.Exists(_fullpath);
        }

        /// <summary>
        /// tests if a file has a extention
        /// </summary>
        public static bool FileExtentionExists(string path)
        {
            return Path.HasExtension(path);
        }

        /// <summary>
        /// if the directory does'nt exist create it
        /// tests for a extention if there is one the filename is striped off
        /// </summary>
        /// <param name="specifiedfolderpath"></param>
        public static void PathCreateDirectory(string _fullpath)
        {
            if (Path.HasExtension(_fullpath))
            {
                _fullpath = PathRemoveFileName(_fullpath);
            }
            if (System.IO.Directory.Exists(_fullpath) == false)
            {
                System.IO.Directory.CreateDirectory(_fullpath);
            }
        }

        /// <summary>
        /// returns true or false if the path or file has a extension
        /// </summary>
        public static bool PathExtentionExists(string path)
        {
            return Path.HasExtension(path);
        }

        /// <summary>
        /// check if folder exists works on full filepaths as well
        /// </summary>
        public static bool PathToFolderExists(string path)
        {
            if (Path.HasExtension(path))
            {
                path = PathRemoveFileName(path);
            }
            return System.IO.Directory.Exists(path);
        }

        /// <summary>
        /// strip a filename off a path
        /// and return just the path without the filename
        /// </summary>
        public static string PathRemoveFileName(string filenameorfolder)
        {
            return Path.GetDirectoryName(filenameorfolder);
        }

        /// <summary>
        /// strip a filename extention off
        /// </summary>
        public static string PathRemoveExtention(string pathorfilename)
        {
            return Path.GetFileNameWithoutExtension(pathorfilename);
        }

        /// <summary>
        /// returns path to mydocuments folder
        /// </summary>
        /// <returns></returns>
        public static string PathToDocumentsFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// simply returns the get current enviroment path string
        /// this is typically the application folder of the executable  
        /// </summary>
        public static string PathToCurrentEnviroment()
        {
            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// <para>get the fullpath of the parent directory of the given path</para>
        /// <para>if the path is a full path to a file the filename will be striped off before the operation proccede's</para>
        /// </summary>
        /// <param name="path">the path to get the parent from</param>
        /// <returns>the parent directory if thier is none it just passes back out the inputed directory</returns>
        public static string PathGetParentDirectory(string path)
        {
            if (FileExtentionExists(path))
            {
                path = PathRemoveFileName(path);
            }
            DirectoryInfo d = Directory.GetParent(path);
            if (d != null)
            {
                return d.FullName;
            }
            else
            {
                return path;
            }
        }       

        /// <summary>
        /// <para>get the fullpath of the parent directory of the given path</para>
        /// <para>if the path is a full path to a file the filename will be striped off before the operation proccede's</para>
        /// </summary>
        /// <param name="path">the path to get the parent from</param>
        /// <param name="numberOfStepsBack">the number of times we recursively execute the operation</param>
        /// <returns>the parent directory if thier is none it just passes back out the inputed directory</returns>
        public static string PathGetParentDirectory(string path, int numberOfStepsBack)
        {
            for (int i = 0; i < numberOfStepsBack; i++)
            {
                var test = PathGetParentDirectory(path);
                if (Directory.Exists(test))
                    path = test;
            }
            return path;
        }
        
        /// <summary>
        /// this method changes the parent directory of the file
        /// note filenames without extensions may not be handled by this method correctly
        /// and may only strip off the filename
        /// 
        /// </summary>
        public static string PathReplaceParentDirectory(string fullpath, string foldername)
        {
            string newpath = "";
            string filename = FileNameFromPath(fullpath);
            if (FileExtentionExists(fullpath))
            {
                newpath = PathRemoveFileName(fullpath);
            }
            DirectoryInfo d = Directory.GetParent(newpath);
            if (d != null)
            {
                newpath = d.FullName;
                return CombinePath(newpath, foldername, filename);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// takes a full file path or path with a filename and extension and backs up the directory
        /// it is in, the returned path will be the same but up one directory level
        /// ie  a/b/filename  will be a/filename
        /// </summary>
        public static string PathFullFilePathBackwards(string fullpath)
        {
            string pfile = "";
            string path = "";
            if (FileExtentionExists(fullpath))
            {
                pfile = FileNameFromPath(fullpath);
                path = PathRemoveFileName(fullpath);
                path = PathGetParentDirectory(path);
                path = CombinePath(path, pfile);
            }
            else
            {
                return null;
            }
            return path;
        }

        /// <summary>
        /// returns the filename in the path
        /// with the extension
        /// </summary>
        public static string FileNameFromPath(string apath)
        {
            return Path.GetFileName(apath);
        }

        /// <summary>
        /// returns the filename in the path
        /// without the extension
        /// </summary>
        public static string FileNameFromPathWithoutExtention(string apath)
        {
            return Path.GetFileNameWithoutExtension(Path.GetFileName(apath));
        }

        /// <summary>
        /// just get the extention off the filenames path
        /// </summary>
        public static string FileExtention(string filename_or_path)
        {
            return Path.GetExtension(filename_or_path);
        }

        /// <summary>
        /// just get the extention off the filenames path
        /// </summary>
        public static string FileExtentionWithoutDotFileSeperator(string filename_or_path)
        {
            // yank dot of ext  !!!note  //this doesn't work !!!extnodot.Replace(".", ""); 
            string _ext = Path.GetExtension(filename_or_path);
            string[] parts = _ext.Split(new char[] { '.' });
            int n = parts.Length - 1;
            if (n <= 0) { return null; }
            else
            {
                return parts[parts.Length - 1];
            }
            //if(extnodot.Contains(".")){extnodot=extnodot.Remove(0,1);} // this works but isn' as safe
        }

        /// <summary>
        /// gets the files and folders within the specified _fullpath
        /// out's the starting index of were the files begin
        /// </summary>
        /// <param name="_path">the full path to were we search for folders and files</param>
        /// <param name="startindexoffiles">out's the starting index of were the files begin</param>
        /// <returns>returns a string array of full path entries in the directory</returns>
        public static string[] GetFoldersAndFilesInPathDirectory(string _path, out int startindexoffiles)
        {
            string[] A = GetSubFolderNamesInDirectory(_path);
            string[] B = Directory.GetFiles(_path);
            string[] C = new string[A.Length + B.Length];
            for (int a = 0; a < A.Length; a++)
            {
                C[a] = A[a];
            }
            for (int b = 0; b < B.Length; b++)
            {
                C[b + A.Length] = B[b];
            }
            startindexoffiles = A.Length;
            return C;
        }

        /// <summary>
        /// <para>ok this is a nice little function that will return either names or fullpaths within a folder </para>
        /// <para>it will also order the folders first and the files second </para>
        /// <para>the out integer value will allow you to get the index to the start of the files in the list</para>
        /// </summary>
        /// <param name="fullpath">full path to folder to search</param>
        /// <param name="get_these_exts">look for following extentions</param>
        /// <param name="should_we_ret_fullpaths">get back full paths(true) or just names (false)</param>
        /// <param name="out_the_startindex_to_files_">the int you pass here will be changed to the start index in the returned array of the files</param>
        /// <returns>a array containing the file or folders at the specified path</returns>
        public static string[] GetFoldersAndFilesInPathDirectory(string fullpath, string get_this_extension, bool should_we_ret_fullpaths, out int out_the_startindex_to_files_)
        {
            string[] A = GetSubFolderNamesOrFullPathsInDirectory(fullpath, should_we_ret_fullpaths);
            string[] B = GetFileNamesInFolder(fullpath, get_this_extension, should_we_ret_fullpaths);//Directory.GetFiles(path);
            string[] C = new string[A.Length + B.Length];
            for (int a = 0; a < A.Length; a++)
            {
                C[a] = A[a];
            }
            for (int b = 0; b < B.Length; b++)
            {
                C[b + A.Length] = B[b];
            }
            out_the_startindex_to_files_ = A.Length;
            return C;
        }

        /// <summary>
        /// <para>ok this is a nice little function that will return either names or fullpaths within a folder </para>
        /// <para>it will also order the folders first and the files second </para>
        /// <para>the out integer value will allow you to get the index to the start of the files in the list</para>
        /// </summary>
        /// <param name="fullpath">full path to folder to search</param>
        /// <param name="get_these_exts">look for following extentions</param>
        /// <param name="should_we_ret_fullpaths">get back full paths(true) or just names (false)</param>
        /// <param name="out_the_startindex_to_files_">the int you pass here will be changed to the start index in the returned array of the files</param>
        /// <returns>a array containing the file or folders at the specified path</returns>
        public static string[] GetFoldersAndFilesInPathDirectory(string fullpath, string[] get_these_extensions, bool should_we_ret_fullpaths, out int out_the_startindex_to_files_)
        {
            string[] A = GetSubFolderNamesOrFullPathsInDirectory(fullpath, should_we_ret_fullpaths);
            string[] B = GetFileNamesInFolderWithExtensions(fullpath, get_these_extensions, should_we_ret_fullpaths);//Directory.GetFiles(path);
            string[] C = new string[A.Length + B.Length];
            for (int a = 0; a < A.Length; a++)
            {
                C[a] = A[a];
            }
            for (int b = 0; b < B.Length; b++)
            {
                C[b + A.Length] = B[b];
            }
            out_the_startindex_to_files_ = A.Length;
            return C;
        }

        /// <summary>
        /// get all the files in a path
        /// </summary>
        public static string[] GetFilesInFolder(string path)
        {
            return GetFileNamesInFolder(path, "ALL");
        }

        /// <summary>
        /// this gets the files of a given type (ie.. png ect...) in a specific directory
        /// this version is overloaded to allow for a specific path optionally it can find other files in the same dir as a filepath
        /// </summary>
        /// <param name="path">the directory or full path to the folder or were the files exist</param>
        /// <param name="filetype">use All all Any any to get back all the file names in the folder</param>
        /// <returns>a string array of the files in the folder by name or by pathname</returns>
        public static string[] GetFileNamesInFolder(string path, string filetype)
        {
            bool getfullpathstoo = false;
            string[] namearray;
            if (filetype != null && path != null)
            {
                if (FileExists(path))
                {
                    path = PathRemoveFileName(path);
                }
                //
                if (
                    filetype == null || filetype == ""
                    ||
                    string.Compare(filetype, "ALL", true) == 0 || string.Compare(filetype, ".all", true) == 0 || string.Compare(filetype, ".All", true) == 0
                    ||
                    string.Compare(filetype, "ANY", true) == 0 || string.Compare(filetype, ".any", true) == 0 || string.Compare(filetype, ".Any", true) == 0
                   )
                {
                    namearray = Directory.GetFiles(path);
                }
                else
                {
                    filetype = filetype.Replace(".", "");
                    namearray = Directory.GetFiles(path, "*." + filetype);
                }

                //foreach (var s in namearray)
                //    Console.WriteLine(s);

                if (getfullpathstoo)
                {
                    return namearray;
                }
                else
                {
                    int i = 0;
                    while (i < namearray.Length)
                    {
                        namearray[i] = Path.GetFileName(namearray[i]);
                        i++;
                    }
                    return namearray;
                }
            }
            else
            {
                namearray = new string[0];
                return namearray;
            }
        }

        /// <summary>
        /// gets the files in a dir by the fullfilepath
        /// </summary>
        public static string[] GetFilesFullPathNamesInFolder(string path, string filetype)
        {
            return GetFileNamesInFolder(path, filetype, true);
        }

        /// <summary>
        /// this gets the files of a given type (ie.. png ect...) in a specific directory
        /// this version is overloaded to allow for a specific path optionally it can find other files in the same dir as a filepath
        /// </summary>
        /// <param name="path">the directory or full path to the folder or were the files exist</param>
        /// <param name="filetype">use All all Any any to get back all the file names in the folder</param>
        /// <returns>a string array of the files in the folder by name or by pathname</returns>
        public static string[] GetFileNamesInFolderWithoutExt(string path, string filetype)
        {
            bool getfullpathstoo = false;
            string[] namearray;
            if (filetype != null && path != null)
            {
                if (FileExists(path))
                {
                    path = PathRemoveFileName(path);
                }
                //
                if (
                    filetype == "" || filetype == null
                    ||
                    string.Compare(filetype, "ALL", true) == 0 || string.Compare(filetype, ".all", true) == 0 || string.Compare(filetype, ".All", true) == 0
                    ||
                    string.Compare(filetype, "ANY", true) == 0 || string.Compare(filetype, ".any", true) == 0 || string.Compare(filetype, ".Any", true) == 0
                   )
                {
                    namearray = Directory.GetFiles(path);
                }
                else
                {
                    filetype = filetype.Replace(".", "");
                    namearray = Directory.GetFiles(path, "*." + filetype);
                }

                if (getfullpathstoo)
                {
                    return namearray;
                }
                else
                {
                    int i = 0;
                    while (i < namearray.Length)
                    {
                        namearray[i] = Path.GetFileName(namearray[i]);
                        namearray[i] = PathRemoveExtention(namearray[i]);
                        i++;
                    }
                    return namearray;
                }
            }
            else
            {
                namearray = new string[0];
                return namearray;
            }
        }       

        /// <summary>
        /// this gets the files of a given type (ie.. png ect...) in a specific directory
        /// this version is overloaded to allow for a specific path 
        /// </summary>
        /// <param name="path">the directory or full path to the folder were the files exist</param>
        /// <param name="filetype">use All all Any any to get back all the file names in the folder</param>
        /// <param name="getfullpathstoo">true returns the fully qualified path in the string array false returns just the file name</param>
        /// <returns>a string array of the files in the folder by name or by pathname</returns>
        public static string[] GetFileNamesInFolder(string path, string filetype, bool getfullpathstoo)
        {
            string[] namearray;
            if (filetype != null && path != null)
            {
                //if(File.Exists(path))
                if (FileExists(path))
                {
                    path = PathRemoveFileName(path);
                }
                //
                if (
                    filetype == "" || filetype == null 
                    ||
                    string.Compare(filetype, "ALL", true) == 0 || string.Compare(filetype, ".ALL", true) == 0
                    ||
                    string.Compare(filetype, "Any", true) == 0 || string.Compare(filetype, ".Any", true) == 0
                   )
                {
                    namearray = Directory.GetFiles(path);
                }
                else
                {
                    filetype = filetype.Replace(".", "");
                    namearray = Directory.GetFiles(path, "*." + filetype);
                }

                if (getfullpathstoo)
                {
                    return namearray;
                }
                else
                {
                    int i = 0;
                    while (i < namearray.Length)
                    {
                        namearray[i] = Path.GetFileName(namearray[i]);
                        i++;
                    }
                    return namearray;
                }
            }
            else
            {
                namearray = new string[0];
                return namearray;
            }
        }

        /// <summary>
        /// this method returns files in the specified folder of the types contained in the string array
        /// which is passed to this method if all or any is a string in the array all files will be returned
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filetypes">use ALL or Any to get all or any filetypes</param>
        /// <returns></returns>
        public static string[] GetFileNamesInFolderWithExtensions(string path, string[] filetypes, bool getfullpathstoo)
        {
            List<string> matchinglist = new List<string>();
            string[] namearray;
            //
            if (Path.HasExtension(path))
            {
                path = PathRemoveFileName(path);
            }

            // fix for the case were any or all is specified we were not taking that into account before               
            bool anyall_result = false;
            for (int i = 0; i < filetypes.Length; i++)
            {
                filetypes[i] = filetypes[i].Replace(".", "");// strip period off first
                if (string.Compare(filetypes[i], "any", true) == 0 || string.Compare(filetypes[i], "all", true) == 0)
                {
                    anyall_result = true;
                    namearray = Directory.GetFiles(path);
                    for (int n = 0; n < namearray.Length; n++)
                    {
                        matchinglist.Add(namearray[n]);
                    }
                    i = filetypes.Length; // might as well break out now
                }
            }
            if (anyall_result == false)
            {
                for (int ft = 0; ft < filetypes.Length; ft++)
                {
                    filetypes[ft] = filetypes[ft].Replace(".", ""); // strip period off first
                                                                    // specific
                    namearray = Directory.GetFiles(path, "*." + filetypes[ft]);// replace period with invariant case search also
                    for (int n = 0; n < namearray.Length; n++)
                    {
                        matchinglist.Add(namearray[n]);
                    }
                }
            }
            namearray = matchinglist.ToArray();
            //
            if (getfullpathstoo)
            {
                return namearray;
            }
            else
            {

                int i = 0;
                while (i < namearray.Length)
                {
                    namearray[i] = Path.GetFileName(namearray[i]);
                    i++;
                }
                return namearray;
            }
        }

        /// <summary>
        /// returns a string array of the subfolders in a directory at the searchpath specified 
        /// the bool set to true returns only the subdirectorys name without the path
        /// its not recursive
        /// </summary>
        /// <param name="searchpath"></param>
        /// <param name="justreturnname"></param>
        /// <returns></returns>
        public static string[] GetSubFolderNamesOrFullPathsInDirectory(string path, bool returnfullpathstoo)
        {
            string[] dirpatharray = Directory.GetDirectories(path);
            string[] subdirname = new string[dirpatharray.Length];
            int i = 0;
            while (i < dirpatharray.Length)
            {
                string[] brokenpath = dirpatharray[i].Split('\\');
                subdirname[i] = brokenpath.Last();
                i++;
            }
            // now i can return the full path or just the names of the subdirectorys 
            if (returnfullpathstoo)
            {
                return dirpatharray;
            }
            else
            {
                return subdirname;
            }
        }

        /// <summary>
        /// returns a string array of the subfolders in a directory 
        /// at the searchpath specified 
        /// returns only the subdirectorys name with or without out the path
        /// </summary>
        /// <param name="searchpath"></param>
        /// <param name="justreturnname"></param>
        /// <returns>returns only the subdirectorys names without the path</returns>
        public static string[] GetSubFolderNamesInDirectory(string path)
        {
            string[] dirpatharray = Directory.GetDirectories(path);
            string[] subdirname = new string[dirpatharray.Length];
            int i = 0;
            while (i < dirpatharray.Length)
            {
                string[] brokenpath = dirpatharray[i].Split('\\');
                subdirname[i] = brokenpath.Last();
                i++;
            }
            return subdirname;
        }

        /// <summary>
        /// get root drive paths C D E ect...
        /// </summary>
        /// <returns></returns>
        public static string[] GetRootDrivePaths()
        {
            return Directory.GetLogicalDrives();
        }

        /// <summary>
        /// gets the folder heirarchy to a array for the specified path
        /// strips off the filename first before testing
        /// </summary>
        /// <param name="apath"></param>
        /// <returns></returns>
        public static string[] GetFolderHeirarchy(string apath)
        {
            apath = PathRemoveFileName(apath);
            string[] structure = apath.Split(Path.DirectorySeparatorChar);
            return structure;
        }

        public static string GetPartialDirectoryFromFilePathEnd(string full_filepath, string foldertarget)
        {
            char ds = Path.DirectorySeparatorChar;
            char ads = Path.AltDirectorySeparatorChar;
            char ps = Path.PathSeparator;
            string[] temp = full_filepath.Split(new char[] { ds, ads, ps }, StringSplitOptions.None);
            int j = 0;
            bool result = false;
            for (int i = 0; i < temp.Length; i++)
            {
                if (true == CompareStringArray(temp, foldertarget, false, out j))
                {
                    result = true;
                }
            }
            string newstr = "";
            if (result == true)
            {
                for (int k = j; k < temp.Length; k++)
                {
                    newstr = CombinePath(newstr, temp[k]);
                }
                return newstr;
            }
            else
            {
                return full_filepath;
            }
        }

        /// <summary>
        /// fixes a double period in a path or file name typically when manually changing exts i had this problem
        /// </summary>
        /// <param name="pathorfilename"></param>
        /// <returns></returns>
        public static string FixDoublePeriodInExtension(string pathorfilename)
        {
            return pathorfilename.Replace("..", ".");
        }

        public static void WriteStringToFile(string path, string text)
        {
            File.WriteAllText(path, text);
        }

        /// <summary>
        /// using System; 
        /// using System.IO;
        /// This is a little cheating method so you don't have to type out everything in the content folder.
        /// you get a temp.txt file in my documents that is formatted to copy paste into game 1
        /// </summary>
        public static void QuickWriteOutContentFolderForCopyPaste(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            QuickWriteOutContentFolderForCopyPaste(null, content);
        }

        /// <summary>
        /// using System; 
        /// using System.IO;
        /// This is a little cheating method so you don't have to type out everything in the content folder.
        /// you get a temp.txt file in my documents that is formatted to copy paste into game 1
        /// </summary>
        public static void QuickWriteOutContentFolderForCopyPaste(string folder, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            string envpath = Environment.CurrentDirectory;
            string contentpath = content.RootDirectory;
            string path = contentpath;
            if (folder != null && folder != "")
                Path.Combine(contentpath, folder);
            content.RootDirectory = path;
            string FileSearchPath = Path.Combine(envpath, path);
            string[] files = MgPathFolderFileOps.GetFileNamesInFolderWithoutExt(FileSearchPath, "All");
            content.RootDirectory = contentpath;

            string itemsprefix = "t2d_";

            string msg = "\n#region Texture2Ds programatically listed";
            msg += "\n";
            for (int i = 0; i < files.Length; i++)
            {
                msg += "\n" + "public Texture2D " + itemsprefix + files[i] + ";";
            }
            msg += "\n\n #endregion";
            msg += "\n";

            // the part were its loaded
            msg += "\n" + "public void LoadStuff(){ \n";
            msg += "\n Content.RootDirectory = " + '@' + '"' + path + '"' + ";";
            msg += "\n";
            for (int i = 0; i < files.Length; i++)
            {
                msg += "\n" + itemsprefix + files[i] + " = Content.Load<Texture2D>( " + '"' + files[i] + '"' + ");";
            }
            msg += "\n Content.RootDirectory = " + '@' + '"' + contentpath + '"' + ";";
            msg += "\n} \n";
            //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            string fullpath = MgPathFolderFileOps.CombinePath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "temp.txt");
            MgPathFolderFileOps.WriteStringToFile(fullpath, msg);
            Process.Start(fullpath);
        }


        /// <summary>
        /// quick get of the requested frame 
        /// simply returns the method name
        /// that was called under the frame requested
        /// </summary>
        /// <returns></returns>
        //public static string getStackInfoFrame(int whatframe)
        //{
        //    StackTrace st = new StackTrace(true);
        //    if (whatframe < st.FrameCount)
        //    {
        //        StackFrame sf = st.GetFrame(whatframe);
        //        string filename = BxPathing.getFileNameFromPath(sf.GetFileName());
        //        return ("StackFrame[" + whatframe + "]" + " " + filename + "." + sf.GetMethod() + " Line" + sf.GetFileLineNumber());
        //    }
        //    else
        //    {
        //        StackFrame sf = st.GetFrame(0);
        //        string filename = BxPathing.getFileNameFromPath(sf.GetFileName());
        //        return ("StackFrame[" + whatframe + "]" + " " + filename + "." + sf.GetMethod() + " Line" + sf.GetFileLineNumber());
        //    }
        //}
        ///// <summary>
        ///// quick get of the stack frames
        ///// </summary>
        ///// <returns></returns>
        //public static string getStackInfoFrames()
        //{
        //    string s = "stack info : \n";
        //    StackTrace st = new StackTrace(true);
        //    int whatframe = 0;
        //    while (whatframe < st.FrameCount)
        //    {
        //        StackFrame sf = st.GetFrame(whatframe);
        //        s += "StackFrame[" + whatframe + "]" + " " + BxPathing.getFileNameFromPath(sf.GetFileName()) + "." + sf.GetMethod() + " Line" + sf.GetFileLineNumber();
        //        whatframe++;
        //    }
        //    return s;
        //}

    }
}
