using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;


namespace ADGroup
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Write(UserGroupSearch("gg_BRA08_Sistemas_BI_Team", false));
            Console.ReadKey();
        }

        public static List<string> UserGroupSearch(string q, bool isUser)
        {
            /**
             ** Parametros para conexão ao seu AD
             ** @param[0] = Corresponde ao seu dominio
             ** @param[1] = Usuario utilizado para o login no AD
             ** @param[2] = Senha do usuario utilizado para o login no AD
             **/
            string[] param = { "domain", "user", "password" };

            //Aqui criamos uma lista de string para o retorno das informações
            List<string> ret = new List<string>();

            /**
             ** Criamos os objetos de conexão e os Objetos de pesquisa para usuario e senha passando como parametro de pesquisa 
             ** a informação que vira no input, no caso nosso parametro é o "q", as informações para pesquisa devem ser o 
             ** ususario de login ou o grupo de AD, caso seja informado o usuario, retornara todos os grupos ao qual ele
             ** pertence, caso contrario, retornara todos os usarios vinculado ao grupo informado, o parametro isUser é usado
             ** para alternar entre usuario ou grupo no momento do envio da informação.
             ** 
             ** @q = Query que recebera as informações do input 
             **/
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, param[0], param[1], param[2]);
            GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, q);
            UserPrincipal usr = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, q);

            if ( grp != null && !isUser)
            {
                foreach ( Principal g in grp.GetMembers( false ) )
                {
                    if ( g.StructuralObjectClass == "user" )
                    {
                        var user = ( UserPrincipal )g;
                        if ( user != null )
                        {
                            ret.Add( user.Name );
                        }
                    }
                }

                grp.Dispose();
                ctx.Dispose();
            }
            else if ( usr != null && isUser)
            {
                foreach ( Principal u in usr.GetGroups() )
                {
                    if ( u.StructuralObjectClass == "group" )
                    {
                        var group = ( GroupPrincipal )u;
                        if ( group != null )
                        {
                            ret.Add( group.Name );
                        }
                    }
                }

                usr.Dispose();
                ctx.Dispose();
            }

            return ret;
        }
    }
}
