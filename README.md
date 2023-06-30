## Une api c# asp.net pour l'application SyncFood

### SyncFood api sert d'interface entre l'application " front-end" SyncFood et la base de donnée

## Mise en place de l'api

### Cloner le projet
```
git clone https://github.com/SyncFoodTeam/SyncFood-api
```
### Compiler le projet en version de production
```
dotnet publish --configuration Release
```

## Mise en place d'un proxy inversé avec nginx
Pour fonctionner sur le même serveur que la partie front vous aurez besoin de paramétrer un proxy inversé avec un serveur web de production  
Nous avons décidé d'utiliser nginx  

### Modifier le fichier nginx.conf et rajoutez-y les lignes suivantes
```
location /api {
  proxy_pass http://localhost:5104;
}
```
(Modifier le port 5104 avec celui de l'api si vous l'avez modifié)  
Pour plus d'information sur le proxy inversé  
https://docs.nginx.com/nginx/admin-guide/web-server/reverse-proxy/

"" Compte Administrateur par défaut
Par défaut un utilisateur Admin est créé automatiquement
Email : admin@admin
Mot de passe : adminadmin

Il est fortement recommandé de modifier l'email et le mot de passe de ce compte avant une mise en production
