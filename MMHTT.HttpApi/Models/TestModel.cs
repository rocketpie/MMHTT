using MMHTT.Domain;
using MMHTT.HttpApi.Controllers;
using MMHTT.HttpApi.Helper;

namespace MMHTT.HttpApi.Models
{
  public class TestModel
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public TestManager TestManager { get; set; }
    public LogBuffer Log { get; set; }
  }
}