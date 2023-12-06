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

        Assert.AreEqual(4, service.GetDagligFaste().Count());

        service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId,
            2, 2, 1, 0, DateTime.Now, DateTime.Now.AddDays(3));

        Assert.AreEqual(5, service.GetDagligFaste().Count());
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
        throw new ArgumentNullException();
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

    [TestMethod]

    public void Dagligfastdoegndosis()

    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DateTime startDate = new DateTime(2023, 10, 20);
        DateTime slutDate = new DateTime(2023, 10, 31);

        var dagligfast1 = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 2, 1, 0, 1, startDate, slutDate);
        var dagligfast2 = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, -1, 2, 0, 1, startDate, slutDate);
        var dagligfast3 = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 0, 0, 0, 0, startDate, slutDate);

        Assert.AreEqual(4, dagligfast1.doegnDosis());
        Assert.AreEqual(2, dagligfast2.doegnDosis());
        Assert.AreEqual(0, dagligfast3.doegnDosis());


    }

    [TestMethod]
    public void PNDoegndosis()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DateTime startDate = new DateTime(2023, 11, 24);
        DateTime slutDate = new DateTime(2023, 11, 30);

        // Creating PN instance using service.OpretPN
        var PNdoegnDosis = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 2, startDate, slutDate);

        // Act
        double result = PNdoegnDosis.doegnDosis();

        // Assert
        Assert.AreEqual(5, result);
    }

}

