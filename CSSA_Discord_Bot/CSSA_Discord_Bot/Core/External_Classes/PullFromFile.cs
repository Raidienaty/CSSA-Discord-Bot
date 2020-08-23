namespace CSSA_Discord_Bot
{
    class PullFromFile
    {
        public PullFromFile() {}

        public string FilePull(string name)
        {
            string file = System.IO.File.ReadAllText(name);
            
            if (file != null)
                return file;
            else
                return "Empty";
        }
    }
}