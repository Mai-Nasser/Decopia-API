using Rules_Engine_API.Models;

namespace Rules_Engine_API.Services
{
    public interface IRulesEngine
    {
        List<DetectionResult> Analyze(string payload);

    }
}
