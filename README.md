Para crear las entidades a partir de la BD en MySql el comando es el siguiente.
Scaffold-DbContext "Server=localhost;Database=XXXXXX;User=XXXXXX;Password=XXXXXX;" Pomelo.EntityFrameworkCore.MySql -OutputDir XXXXX

Antes de hacer eso hay que instalar EntityFramework, tanto en el proyecto principal (api) como en el de entidades, a esto sumarle "Pomelo.EntityFrameworkCore.MySql"
y Pomelo.EntityFrameworkCore.MySql.Design, despues de esto, hacer referencia en el proyecto principal (api) al de Entidades. 
Ahora si, ir a herramientas, administrador de paquetes NuGet, y abrir la consola, una vez ahí, en proyecto predeterminado, seleccionar el de Entidades, pegar el comando con los datos correspondientes
a nuestra base de datos. Recordar que luego de "-OutputDir" en el comando, debe ir el nombre de la carpeta que creamos dentro del proyecto entidades, si no esta creado, lo creara automaticamente.
Si no se pone esta directiva, se creara el archivo suelto en el proyecto. Recordar NO ponerle a la carpeta: "DbContext", para evitar confliectos.

Instalar tambien System.IdentityModel.Tokens.Jwt.
