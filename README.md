# Salvation - Partie Niko

## Syst�me de Tick

Le syst�me de tick (*Ticking System*) est le syst�me temporel utilis� par le jeu.
Le syst�me est modulable, c'est � dire qu'on peut d�finir le nombre de secondes entre chaque tick pour acc�l�rer ou ralentir le jeu.

## Co�ts et Ressources

Un b�timent peut contenir 3 diff�rents types de co�ts :
- Co�t de construction (*Construction Costs*) : ce co�t intervient lors de la construction du b�timent.
- Co�t par tick (*Tick Costs*) : ce co�t intervient � chaque tick du jeu.
- Production par tick (*Tick Productions*) : cette production intervient � chaque tick du jeu.

Chaque co�t peut contenir une � plusieurs ressources parmis les suivantes d�finies dans Salvation :
- Population (*People*) : d�fini le nombre de personnes � disposition.
- Energie (*Energy*) : d�fini la quantit� d'�nergie disponible.
- Nourriture (*Food*) : d�fini la quantit� de nourriture � disposition.

## Capacit� d'un b�timent (WIP)

La capacit� d'un b�timent d�termine le nombre de personnes pouvant y �tre affect�.

Plus un b�timent contient de personnes, mieux il fonctionnera. **(?)**  
Un b�timent ne contenant aucune personne sera d�sert� et donc ne produira et ne co�tera rien. **(?)**

## Cr�ation d'un nouveau b�timent dans l'�diteur

Pour cr�er un nouveau b�timent il faut commencer par cr�er un nouveau prefab dans les assets.  
**Clic droit dans le dossier *Assets* > Create > Prefab** puis lui donner le nom du nouveau b�timent.

Une fois le prefab cr��, entrer dans sa fen�tre d'�dition en double cliquant dessus, vous avez maintenant acc�s au prefab.  
Il suffit maintenant de lui indiquer qu'il est un b�timent, pour ce faire on va lui ajouter un nouveau composant :  
**Add Component > Building**

Le composant *Building* vous permet de configurer les diff�rents co�ts de ce b�timent.

Pour ajouter un bouton cr�ant ce nouveau b�timent, il faut se rendre dans l'objet *Canvas* et y cr�er un nouveau bouton :
**Click droit sur Canvas > UI > Button** et nommer ce bouton.

Le texte du bouton est contenu en enfant du bouton cr��, quand � lui le bouton contient un composant *Button*.
Ce composant *Button* contient le champs *On Click ()* qui nous int�resse.  
Clicker sur le **+** pour ajouter un nouvel appel, laisser *Runtime Only*, l'objet � appeler est l'*Input Manager* de la **Scene** et la fonction est :  
**InputManager > Construct (GameObject)**

Un nouveau champs est apparu contenant le *GameObject* � passer en param�tre, il faut donc s�lectionner le b�timent cr�� dans l'onglet **Assets**.

Bravo, vous avez cr�� un nouveau b�timent !