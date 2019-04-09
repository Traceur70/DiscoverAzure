using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eut.Entity.Common
{
    public interface IEutException
    {

        //
        // Résumé :
        //     Obtient l'instance System.Exception qui a provoqué l'exception actuelle.
        //
        // Retourne :
        //     Objet décrivant l'erreur qui a provoqué l'exception actuelle. La propriété System.Exception.InnerException
        //     retourne la même valeur que celle transmise au constructeur System.Exception.#ctor(System.String,System.Exception),
        //     ou null si la valeur de l'exception interne n'a pas été fournie au constructeur.
        //     Cette propriété est en lecture seule.
        Exception InnerException { get; }
        //
        // Résumé :
        //     Obtient un message qui décrit l'exception actuelle.
        //
        // Retourne :
        //     Message d'erreur qui explique la raison de l'exception ou bien chaîne vide ("").
        string Message { get; }
    }
}
