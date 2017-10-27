using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToClipboard.Misc
{
    public class Utils
    {

        /// <summary>
        /// Get a unique name for the file name passed in at the destination.
        /// Duplicates have names like, name(2).txt, name(3).txt etc.
        /// </summary>
        /// <param name="value">Not used</param>
        /// <param name="destination"></param>
        /// <returns>full name of file with duplicate number padded on</returns>
        public static string GetUniqueName(FileInfo value, DirectoryInfo destination = null)
        {
            DirectoryInfo fDestination = destination ?? value.Directory;
            var files = fDestination.GetFiles();
            var fileNames = files.Select(f => f.Name);
            string name = GetUniqueName(fileNames, value.Name);
            return Path.Combine(fDestination.FullName, name);
        }

        /// <summary>
        /// Get a unique name for the file name passed in.
        /// Duplicates have names like, name_2.txt, name_3.txt etc.
        /// </summary>
        /// <param name="checkAgainst">List of file names, not full paths</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetUniqueName(IEnumerable<string> checkAgainst, string name)
        {
            string newName = name;
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(name);
            string extension = Path.GetExtension(name);
            int index = 2, number;

            if (LastIsNumber(nameWithoutExtension, '_', out number)
                // if nameWithoutExtension is a whole number, skip this
                && (0 != string.Compare(number.ToString(), nameWithoutExtension)))
            {
                index = number;
                int numberLength = number.ToString().Length + 1; // +1 for underscore delimiter
                // strip off underscore and number to get accurate number for unique name
                nameWithoutExtension = nameWithoutExtension.Substring(0, nameWithoutExtension.Length - numberLength);
            }

            // check for collisions
            while (checkAgainst.Any(s => s.ToLower() == newName.ToLower()))
                // attempt unique name, format: name(index).extension
                newName = string.Format("{1}_{0}{2}",
                    index++,
                    nameWithoutExtension,
                    extension);
            return newName;
        }

        /// <summary>
        /// Is Last Split by delimiter a number?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delim"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool LastIsNumber(string text, char delim, out int value)
        {
            int lastDelim;
            string number = text;
            if (((lastDelim = text.LastIndexOf(delim)) >= 0) && lastDelim != (text.Length - 1))
                number = text.Substring(lastDelim + 1);
            return int.TryParse(number, out value);
        }
    }
}
