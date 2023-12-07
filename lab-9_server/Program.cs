using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

class Server
{
    static void Main()
    {
        const string eventCharName = "$MyVerySpecialEventName$";
        const string eventTerminationName = "$MyVerySpecialEventTerminationName$";
        const string fileShareName = "$MyVerySpecialFileShareName$";

        EventWaitHandle eventChar = new EventWaitHandle(false, EventResetMode.AutoReset, eventCharName);
        EventWaitHandle eventTermination = new EventWaitHandle(false, EventResetMode.AutoReset, eventTerminationName);

        using (MemoryMappedFile fileMapping = MemoryMappedFile.CreateOrOpen(fileShareName, 100, MemoryMappedFileAccess.ReadWrite))
        {
            using (MemoryMappedViewAccessor accessor = fileMapping.CreateViewAccessor())
            {
                while (true)
                {
                    int waitResult = WaitHandle.WaitAny(new WaitHandle[] { eventTermination, eventChar });
                    if (waitResult == 0)
                        break;

                    char character = (char)accessor.ReadByte(0);
                    Console.Write(character);
                }
            }
        }
    }
}