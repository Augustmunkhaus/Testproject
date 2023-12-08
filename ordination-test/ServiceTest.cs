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
        optionsBuilder.UseInMemoryDatabase(databaseName: $"test-database-{DateTime.UtcNow.Ticks}");
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
        throw new ArgumentNullException();
    }



    [TestMethod]
    public void GivDosisTest()
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
        DagligFast dagligfast = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, -1, 2, 0, 1, startDate, slutDate);
        var dagligfast3 = service.OpretDagligFast(patient.PatientId, lm.LaegemiddelId, 0, 0, 0, 0, startDate, slutDate);

        
        Assert.AreEqual(4, dagligfast1.doegnDosis());
        Assert.IsNotNull(dagligfast);
        Assert.AreEqual(0, dagligfast3.doegnDosis());


    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OpretPN_NegativeTal_Test()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DateTime startDate = new DateTime(2023, 10, 20);
        DateTime slutDate = new DateTime(2023, 10, 31);

        var pn1 = service.OpretPN(patient.PatientId, lm.LaegemiddelId, -1, startDate, slutDate);

        Assert.AreEqual(new ArgumentNullException(),pn1.antalEnheder);

    }

    [TestMethod]
    public void OpretPN_ForkertDato_Test()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DateTime startDate = new DateTime(2023, 10, 31);
        DateTime slutDate = new DateTime(2023, 10, 21);

        
        Assert.ThrowsException<ArgumentException>(() =>
        {
            
            service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, startDate, slutDate);
        });
    }
    [TestMethod]
    public void OpretPN_RigtigDato_Test()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDate = new DateTime(2023, 10, 21);
        DateTime slutDate = new DateTime(2023, 10, 31);

        // Act
        var pn1 = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, startDate, slutDate);

        // Assert
        Assert.IsNotNull(pn1);
    }






    [TestMethod]
         public void OpretPN_PlusAntal_Test()
        {
            // Arrange
            Laegemiddel lm = service.GetLaegemidler().First();
            Patient patient = service.GetPatienter().First();

            DateTime startDate = new DateTime(2023, 10, 20);
            DateTime slutDate = new DateTime(2023, 10, 31);

            // Act
            var pn1 = service.OpretPN(patient.PatientId, lm.LaegemiddelId, 1, startDate, slutDate);

            // Assert
            Assert.IsNotNull(pn1);

        }

    [TestMethod]
    public void DagligSkaevdoegndosis_Test()
    {
        Laegemiddel lm = service.GetLaegemidler().First();
        Patient patient = service.GetPatienter().First();

        DateTime startDate = new DateTime(2023, 10, 20);
        DateTime slutDate = new DateTime(2023, 10, 31);

        DagligSkæv dagligSkaev = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, new Dosis[]
        {
        new Dosis(DateTime.Now, 2),
        new Dosis(DateTime.Now.AddHours(4), 4),
        new Dosis(DateTime.Now.AddHours(6), 3),
        new Dosis(DateTime.Now.AddHours(5), 6)
        }, startDate, slutDate);

        

        Assert.AreEqual(15, dagligSkaev.doegnDosis());
        
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DagligSkaevdoegndosisminus_Test()
    {
        Laegemiddel lm = service.GetLaegemidler().First();
        Patient patient = service.GetPatienter().First();

        DateTime startDate = new DateTime(2023, 10, 20);
        DateTime slutDate = new DateTime(2023, 10, 31);

        DagligSkæv dagligSkaev2 = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, new Dosis[]
        {
        new Dosis(DateTime.Now, 3),
        new Dosis(DateTime.Now.AddHours(4), -4),
        new Dosis(DateTime.Now.AddHours(6), 3),
        new Dosis(DateTime.Now.AddHours(5), 2)
        }, startDate, slutDate);


        Assert.AreEqual(new ArgumentNullException(), dagligSkaev2.doegnDosis());
    }
    [TestMethod]
    public void OpretSkæv_ForkertDato_Test()
    {
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();

        DateTime startDate = new DateTime(2023, 10, 31);
        DateTime slutDate = new DateTime(2023, 10, 21);


        Assert.ThrowsException<ArgumentException>(() =>
        {

            service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, new Dosis[]
        {
        new Dosis(DateTime.Now, 3),
        new Dosis(DateTime.Now.AddHours(4), 2),
        new Dosis(DateTime.Now.AddHours(6), 3),
        new Dosis(DateTime.Now.AddHours(5), 2)
        }, startDate, slutDate);
        });
    }
    [TestMethod]
    
    public void OpretSkæv_RigtigDato_Test()
    {
        // Arrange
        Patient patient = service.GetPatienter().First();
        Laegemiddel lm = service.GetLaegemidler().First();
        DateTime startDate = new DateTime(2023, 10, 21);
        DateTime slutDate = new DateTime(2023, 10, 31);

        // Act
        var skæv1 = service.OpretDagligSkaev(patient.PatientId, lm.LaegemiddelId, new Dosis[]
        {
        new Dosis(DateTime.Now, 3),
        new Dosis(DateTime.Now.AddHours(4), 2),
        new Dosis(DateTime.Now.AddHours(6), 3),
        new Dosis(DateTime.Now.AddHours(5), 2)
        }, startDate, slutDate);

        Assert.AreEqual(new DateTime(2023, 10, 21), startDate);
        // Assert
        
    }
}

