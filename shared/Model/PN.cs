using System.ComponentModel.DataAnnotations;

namespace shared.Model;

public class PN : Ordination {
	public double antalEnheder { get; set; }
    public List<Dato> dates { get; set; } = new List<Dato>();

    public PN (DateTime startDen, DateTime slutDen, double antalEnheder, Laegemiddel laegemiddel) : base(laegemiddel, startDen, slutDen) {
		this.antalEnheder = antalEnheder;
	}

    public PN() : base(null!, new DateTime(), new DateTime()) {
    }

    /// <summary>
    /// Registrerer at der er givet en dosis på dagen givesDen
    /// Returnerer true hvis givesDen er inden for ordinationens gyldighedsperiode og datoen huskes
    /// Returner false ellers og datoen givesDen ignoreres
    /// </summary>
    public bool givDosis(Dato givesDen) {
        // TODO: Implement!
        //Completed

        if (givesDen.dato < startDen || givesDen.dato > slutDen)
        {
            return false;
        }
      //  if (antalEnheder < 0)
        //{
          //  return false; // Kan ikke give et negativt antal doser
        //}
        //dates.Add(givesDen);


        return true;
    }

    public override double doegnDosis() {
    	// TODO: Implement!
        //Completed
        double sum = 0;
        if (dates.Count > 0)
        {
            DateTime min = dates.First().dato;
            DateTime max = dates.First().dato;

            foreach (Dato d in dates)
            {
                if(d.dato < min) {
                    min = d.dato;
                }

                if (d.dato > max) {
                max = d.dato;
                }
            }
            return samletDosis() / ((int)(max-min).TotalDays +1);
            //(antal gange ordinationen er anvendt * antal enheder) / (antal dage mellem første og sidste givning)
        }   
        return 0;
    }


    public override double samletDosis() {
        return dates.Count() * antalEnheder;
    }

    public int getAntalGangeGivet() {
        return dates.Count();
    }

	public override String getType() {
		return "PN";
	}
}
