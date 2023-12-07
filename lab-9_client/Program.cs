using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

class Client
{
    static void Main()
    {
        const string eventCharName = "$MyVerySpecialEventName$";
        const string eventTerminationName = "$MyVerySpecialEventTerminationName$";
        const string fileShareName = "$MyVerySpecialFileShareName$";

        EventWaitHandle eventChar = new EventWaitHandle(false, EventResetMode.AutoReset, eventCharName);
        EventWaitHandle eventTermination = new EventWaitHandle(false, EventResetMode.AutoReset, eventTerminationName);

        using (MemoryMappedFile fileMapping = MemoryMappedFile.OpenExisting(fileShareName, MemoryMappedFileRights.ReadWrite))
        {
            using (MemoryMappedViewAccessor accessor = fileMapping.CreateViewAccessor())
            {
                Console.WriteLine("Mapped and shared file, client process\n\nPress <ESC> to terminate...");

                while (true)
                {
                    char character = Console.ReadKey().KeyChar;
                    accessor.Write(0, (byte)character);
                    eventChar.Set();

                    if (character == 27)
                        break;
                }
            }
        }

        eventTermination.Set();
    }
}