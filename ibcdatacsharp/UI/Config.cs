namespace ibcdatacsharp.UI
{
    public static class Config
    {
        public static string[] allowedExtensions = new string[] { ".txt", ".avi", ".csv" };
        public const bool showOnlyInitialPath = true;
        public const string INITIAL_PATH = "C:\\Temp";
        public const int VIDEO_FPS = 30;
        public const int VIDEO_FPS_SAVE = 25;
        public const int FRAME_HEIGHT = 480;
        public const int FRAME_WIDTH = 640;
        public const string csvHeader = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	ACC_X	ACC_Y	ACC_Z	GYR_X	GYR_Y	GYR_Z	MAG_X	MAG_Y	MAG_Z
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x	x	x	x	x	x	x
";
    }
}
