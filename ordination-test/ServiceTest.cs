namespace ordination_test;

using Microsoft.EntityFrameworkCore;

using Service;
using Data;
using shared.Model;

[TestClass]
public class ServiceTest
{
    private DataService service;

    [TestInitialize]
    public void SetupBeforeEachTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrdinationContext>();
        optionsBuilder.UseInMemoryDatabase(databaseName: "test-database");
        var context = new OrdinationContext(optionsBuilder.Options);
        service = new DataService(context);
        service.SeedData();
    }

    [TestMethod]
    public void PatientsExist()
    {
        Assert.IsNotNull(service.GetPatienter());
    }

    [TestMethod]
    public void OpretDagligFast()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        Assert.AreEqual(1, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(2, service.GetDagligFaste().Count());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TestAtKodenSmiderEnException()
    {
        // Herunder skal man så kalde noget kode,
        // der smider en exception.

        // Hvis koden _ikke_ smider en exception,
        // så fejler testen.

        Console.WriteLine("Her kommer der ikke en exception. Testen fejler.");
    }

    

    [TestMethod]
    public void GivDosisTest2()
    {
        // Arrange
        var laegemiddel = new Laegemiddel("TestLaegemiddel", 1.0, 2.0, 3.0, "mg");
        var patient = new Patient("123456789", "TestPatient", 70.0);

        DateTime startDate = new DateTime(2023, 11, 24);
        DateTime slutDate = new DateTime(2023, 11, 27);

        var pnOrdination = new PN(startDate, slutDate, 2, laegemiddel);

        // Act
        bool result1 = pnOrdination.givDosis(new Dato() { dato = new DateTime(2023, 11, 22) }); // dato er udenfor 
        bool result2 = pnOrdination.givDosis(new Dato() { dato = new DateTime(2023, 11, 25) }); // Dato er indenfor
        bool result3 = pnOrdination.givDosis(new Dato() { dato = new DateTime(2023, 11, 28) }); // Dato er udenfor

        // Assert
        Assert.IsFalse(result1); //forventer false fordi datoen er før startdate
        Assert.IsTrue(result2); //forventer true fordi det er indenfor startdate og slutdate 
        Assert.IsFalse(result3); //forventer false fordi datoen er efter slutdate
    }
}