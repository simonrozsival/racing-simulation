using Racing.Mathematics;
using Racing.Mathematics.Splines;
using System.Linq;
using System.Text;

namespace Racing.CircuitGenerator.Output
{
    internal sealed class ImageGenerator
    {
        private readonly int width;
        private readonly int height;
        private readonly Circuit circuit;

        private const string greenColor = "#80bf3e";
        private const string whiteColor = "#e8e8e8";
        private const string redColor = "#fe4f10";
        private const string blackColor = "#494949";
        private const string blueColor = "#3e9ebf";

        public ImageGenerator(int width, int height, Circuit circuit)
        {
            this.width = width;
            this.height = height;
            this.circuit = circuit;
        }

        public string GenerateSvg()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-16\" standalone=\"no\"?>");
            builder.AppendLine($"<svg width=\"{width}px\" height=\"{height}px\" viewBox=\"0 0 {width} {height}\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");

            renderTo(builder);

            builder.AppendLine("</svg>");

            return builder.ToString();
        }

        private void renderTo(StringBuilder builder)
        {
            var centerPath = circuit.CenterLine();

            // background
            builder.AppendLine(backgroundRectSvg());

            // random trees

            // curbs
            builder.AppendLine(pathSvg(centerPath, whiteColor, circuit.Radius * 2.2));
            builder.AppendLine(pathSvg(centerPath, redColor, circuit.Radius * 2.2, dash: (int)(circuit.Radius / 3)));

            // tarmac
            builder.AppendLine(pathSvg(centerPath, blackColor, circuit.Radius * 2));

            // center line
            builder.AppendLine(pathSvg(centerPath, whiteColor, width: circuit.Radius / 10, dash: (int)(circuit.Radius / 2)));

            // start line
            var startLinePoints = circuit.StartLine();
            builder.AppendLine(printStartLine(startLinePoints[0], startLinePoints[1]));

            var n = 1;
            foreach (var waypoint in circuit.WayPoints.Skip(1))
            {
                builder.AppendLine(printWaypoint(waypoint.Position, n++, radius: 0.6 * circuit.Radius));
            }
        }

        private string backgroundRectSvg()
            => $"<rect fill=\"{greenColor}\" x=\"0\" y=\"0\" width=\"{width}\" height=\"{height}\" />";

        private string printCurb(BezierCurve curve, double width)
        {
            var operations = printPathOperations(curve);
            var background = pathSvg(curve, whiteColor, width);
            var dashedLine = pathSvg(curve, redColor, width, dash: 5);
            return $"{background}\n{dashedLine}";
        }

        private string printStartLine(Vector start, Vector end)
            => pathSvg(start, end, whiteColor, width: 6);

        private string printWaypoint(Vector position, int n, double radius)
            => circleSvg(position, radius, color: blueColor)
                + textSvg(position, n.ToString(), color: whiteColor, size: radius);

        private static string pathSvg(Vector start, Vector end, string color, double width, int? dash = null)
            => paghSvg($"M{start.X},{start.Y} L{end.X},{end.Y}", color, width, dash);

        private static string pathSvg(BezierCurve curve, string color, double width, int? dash = null)
            => paghSvg(printPathOperations(curve), color, width, dash);

        private static string paghSvg(string operations, string color, double width, int? dash = null)
        {
            var dashPattern = dash.HasValue ? $"stroke-dasharray=\"{dash},{dash}\"" : string.Empty;
            return $"<path fill=\"none\" stroke=\"{color}\" d=\"{operations}\" stroke-width=\"{width}\" {dashPattern} />";
        }

        private static string circleSvg(Vector position, double radius, string color)
            => $"<circle r=\"{radius}\" cx=\"{position.X}\" cy=\"{position.Y}\" fill=\"{color}\" />";

        private static string textSvg(Vector position, string text, string color, double size)
            => $"<text x=\"{position.X}\" y=\"{position.Y}\" text-anchor=\"middle\" fill=\"{color}\" font-size=\"{size}\" font-family=\"Arial\" dy=\".35em\">{text}</text>";

        private static string printPathOperations(BezierCurve curve)
        {
            var builder = new StringBuilder();
            var start = curve.Segments.First().Start;
            builder.Append($"M{start.X},{start.Y} ");
            foreach (var segment in curve.Segments)
            {
                builder.Append($"C{segment.StartControlPoint.X},{segment.StartControlPoint.Y} {segment.EndControlPoint.X},{segment.EndControlPoint.Y} {segment.End.X},{segment.End.Y} ");
            }
            builder.Append("Z");
            return builder.ToString();
        }
    }
}
