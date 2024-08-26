using System;
using System.Drawing;
using System.Text;

namespace img_to_ascii
{
    class Program
    {
        static void Main(string[] args)
        {
            bool loop = true;

            //Settings
            int scaleFactor = 1;
            bool isScaleDown = true;
            double clampStrength = 7;
            int charHeightRatio = 2;

            while (loop)
            {
                Console.WriteLine("\n\nWhat would you like to do?");
                Console.WriteLine("1. Convert Image");
                Console.WriteLine("2. View/Change Settings");
                Console.WriteLine("3. Quit");

                string input = Console.ReadLine();
                int inputConverted;

                try
                {
                    inputConverted = Convert.ToInt32(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nInput was not a integer. Please try again.");
                    continue;
                }

                switch (inputConverted)
                {
                    case 1:
                        Console.WriteLine("\n\nEnter the image file path:");
                        string imagePath = Console.ReadLine();

                        //Check if the file exists
                        if (!File.Exists(imagePath))
                        {
                            Console.WriteLine("Image file not found.");
                            break;
                        }

                        try
                        {
                            // Load the image
                            Bitmap originalImage = new Bitmap(imagePath);

                            //Convert the image
                            string asciiOutput = ConvertToAscii(originalImage, scaleFactor, isScaleDown, clampStrength, charHeightRatio);

                            //Save the ascii output
                            string outputPath = "Ascii_" + System.IO.Path.GetFileNameWithoutExtension(imagePath) + ".txt";

                            using (StreamWriter writer = new StreamWriter(outputPath))
                            {
                                writer.WriteLine(asciiOutput);
                            }

                            Console.WriteLine("\nAscii saved to: " + outputPath);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("An error occurred: " + e.Message);
                        }

                        break;
                    case 2:
                        bool loopSettings = true;
                        while (loopSettings)
                        {
                            Console.WriteLine("\n\nSettings:");
                            Console.WriteLine("1. Scale Factor:              " + scaleFactor);
                            Console.WriteLine("2. Scale Direction:           " + (isScaleDown ? "Scale Down" : "Scale Up"));
                            Console.WriteLine("3. Clamp Strength:            " + clampStrength);
                            Console.WriteLine("4. Character Height Ratio:    " + charHeightRatio);
                            Console.WriteLine();
                            Console.WriteLine("5. Back");

                            input = Console.ReadLine();

                            try
                            {
                                inputConverted = Convert.ToInt32(input);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("\nInput was not a integer. Please try again.");
                                continue;
                            }

                            switch (inputConverted)
                            {
                                case 1:
                                    Console.WriteLine("What would you like to change the scale factor to?");
                                    input = Console.ReadLine();

                                    try
                                    {
                                        inputConverted = Convert.ToInt32(input);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nInput was not a integer. Please try again.");
                                        continue;
                                    }

                                    scaleFactor = inputConverted;
                                    break;
                                case 2:
                                    Console.WriteLine("Would you like to scale images up or down?");
                                    Console.WriteLine("1. Scale Down");
                                    Console.WriteLine("2. Scale Up");

                                    input = Console.ReadLine();

                                    try
                                    {
                                        inputConverted = Convert.ToInt32(input);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nInput was not a integer. Please try again.");
                                        continue;
                                    }

                                    switch (inputConverted)
                                    {
                                        case 1:
                                            isScaleDown = true;
                                            break;
                                        case 2:
                                            isScaleDown = false;
                                            break;
                                        default:
                                            Console.WriteLine("Input was not valid. Please try again.");
                                            break;
                                    }

                                    break;
                                case 3:
                                    Console.WriteLine("What would you like to change the clamp strength to?");
                                    input = Console.ReadLine();

                                    try
                                    {
                                        inputConverted = Convert.ToInt32(input);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nInput was not a integer. Please try again.");
                                        continue;
                                    }

                                    clampStrength = inputConverted;
                                    break;
                                case 4:
                                    Console.WriteLine("What would you like to change the character height ratio to?");
                                    input = Console.ReadLine();

                                    try
                                    {
                                        inputConverted = Convert.ToInt32(input);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("\nInput was not a integer. Please try again.");
                                        continue;
                                    }

                                    charHeightRatio = inputConverted;
                                    break;
                                case 5:
                                    loopSettings = false;
                                    break;
                                default:
                                    Console.WriteLine("Input was not valid. Please try again.");
                                    break;
                            }
                        }
                        break;
                    case 3:
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Input was not valid. Please try again.");
                        break;
                }
            }
        }

        static string ConvertToAscii(Bitmap original, int scaleFactor, bool isScaleDown, double clampStrength, double charHeightRatio)
        {
            StringBuilder output = new StringBuilder();

            if (isScaleDown)
            {
                // Convert each group of pixels to greyscale
                for (int y = 0; y < original.Height; y += scaleFactor)
                {
                    for (int x = 0; x < original.Width; x += scaleFactor)
                    {
                        double pixelValue = GetGreyscaleAverage(original, x, y, scaleFactor);

                        char character = valueToAscii(pixelValue, clampStrength);

                        for (int i = 0; i < charHeightRatio; i++)
                        {
                            output.Append(character);
                        }
                    }
                    output.Append('\n');
                }
            }
            else
            {
                for (int y = 0; y < original.Height; y++)
                {
                    StringBuilder line = new StringBuilder();
                    for (int x = 0; x < original.Width; x++)
                    {
                        double pixelValue = GetGreyscaleAverage(original, x, y, 1);

                        char character = valueToAscii(pixelValue, clampStrength);

                        for (int i = 0; i < charHeightRatio * scaleFactor; i++)
                        {
                            line.Append(character);
                        }
                    }

                    for (int i = 0; i < scaleFactor; i++)
                    {
                        output.Append(line.ToString());
                        output.Append('\n');
                    }
                }
            }

            return output.ToString();
        }

        static double GetGreyscaleAverage(Bitmap original, int x, int y, int size)
        {
            int countedPixels = 0;
            double total = 0;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (y + i >= original.Height) break;
                    if (x + j >= original.Width) break;

                    countedPixels++;

                    Color originalColor = original.GetPixel(x + j, y + i);
                    total += (originalColor.R * 0.299 + originalColor.G * 0.587 + originalColor.B * 0.114);
                }
            }

            return total / countedPixels;
        }

        static char valueToAscii(double value, double clampStrength)
        {
            string characters = " `.-':_,^=;><+!rc*/z?sLTv)J7(|Fi{C}fI31tlu[neoZ5Yxjya]2ESwqkP6h9d4VpOGbUAKXHm8RD#$Bg0MNWQ%&@";
            double[] weights = [0, 0.0751, 0.0829, 0.0848, 0.1227, 0.1403, 0.1559, 0.185, 0.2183, 0.2417, 0.2571, 0.2852, 0.2902, 0.2919, 0.3099, 0.3192, 0.3232, 0.3294, 0.3384, 0.3609, 0.3619, 0.3667, 0.3737, 0.3747, 0.3838, 0.3921, 0.396, 0.3984, 0.3993, 0.4075, 0.4091, 0.4101, 0.42, 0.423, 0.4247, 0.4274, 0.4293, 0.4328, 0.4382, 0.4385, 0.442, 0.4473, 0.4477, 0.4503, 0.4562, 0.458, 0.461, 0.4638, 0.4667, 0.4686, 0.4693, 0.4703, 0.4833, 0.4881, 0.4944, 0.4953, 0.4992, 0.5509, 0.5567, 0.5569, 0.5591, 0.5602, 0.5602, 0.565, 0.5776, 0.5777, 0.5818, 0.587, 0.5972, 0.5999, 0.6043, 0.6049, 0.6093, 0.6099, 0.6465, 0.6561, 0.6595, 0.6631, 0.6714, 0.6759, 0.6809, 0.6816, 0.6925, 0.7039, 0.7086, 0.7235, 0.7302, 0.7332, 0.7602, 0.7834, 0.8037, 0.9999];

            value /= 255;

            value = 1 - value;

            //Number clamping thing
            value = 0.5 / Math.Atan(0.5 * clampStrength) * Math.Atan(clampStrength * (value - 0.5)) + 0.5;

            for (int i = 0; i < weights.Length; i++)
            {
                if (weights[i] <= value) continue;

                return Math.Abs(weights[i - 1] - value) < Math.Abs(weights[i] - value) ? characters[i - 1] : characters[i];
            }

            return characters[characters.Length - 1];
        }
    }

}
