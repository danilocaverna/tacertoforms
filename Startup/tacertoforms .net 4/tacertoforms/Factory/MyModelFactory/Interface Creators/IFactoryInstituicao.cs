using System.Collections.Generic;
using TaCertoForms.Models;

namespace TaCertoForms.Factory{
    public interface IFactoryInstituicao{
        Instituicao FindInstituicao(int? id);
        List<Instituicao> InstituicaoList();
    }
}