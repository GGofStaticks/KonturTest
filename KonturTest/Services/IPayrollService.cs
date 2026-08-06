using KonturTest.Models;

namespace KonturTest.Services;

public interface IPayrollService
{
    PayrollResult Process(string dataFileName);

    PayrollResult AddItemAndProcess(string name, string surname, string amount, string mount);
}
