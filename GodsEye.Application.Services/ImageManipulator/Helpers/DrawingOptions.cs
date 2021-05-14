namespace GodsEye.Application.Services.ImageManipulator.Helpers
{
    public class DrawingOptions
    {
        public int LineThickness { get; set; }
        public Rgb Color { get; set; } = new Rgb();

        public void Deconstruct(out int lineThickness, out (byte, byte, byte) rgb)
        {
            //deconstruct the object
            Color.Deconstruct(out rgb);

            lineThickness = LineThickness;
        }
    }

    public class Rgb
    {
        public byte Red { get; set; } = 0;

        public byte Green { get; set; } = 0;

        public byte Blue { get; set; } = 0;

        public void Deconstruct(out (byte, byte, byte) rgb)
        {
            rgb = (Red, Green, Blue);
        }
    }
}
