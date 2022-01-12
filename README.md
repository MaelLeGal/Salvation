# Salvation - Partie Niko

## Système de Tick

Le système de tick (*Ticking System*) est le système temporel utilisé par le jeu.
Le système est modulable, c'est à dire qu'on peut définir le nombre de secondes entre chaque tick pour accélérer ou ralentir le jeu.

## Coûts et Ressources

Un bâtiment peut contenir 3 différents types de coûts :
- Coût de construction (*Construction Costs*) : ce coût intervient lors de la construction du bâtiment.
- Coût par tick (*Tick Costs*) : ce coût intervient à chaque tick du jeu.
- Production par tick (*Tick Productions*) : cette production intervient à chaque tick du jeu.

Chaque coût peut contenir une à plusieurs ressources parmis les suivantes définies dans Salvation :
- Population (*People*) : défini le nombre de personnes à disposition.
- Energie (*Energy*) : défini la quantité d'énergie disponible.
- Nourriture (*Food*) : défini la quantité de nourriture à disposition.

## Capacité d'un bâtiment (WIP)

La capacité d'un bâtiment détermine le nombre de personnes pouvant y être affecté.

Plus un bâtiment contient de personnes, mieux il fonctionnera. **(?)**  
Un bâtiment ne contenant aucune personne sera déserté et donc ne produira et ne coûtera rien. **(?)**

## Création d'un nouveau bâtiment dans l'éditeur

Pour créer un nouveau bâtiment il faut commencer par créer un nouveau prefab dans les assets.  
**Clic droit dans le dossier *Assets* > Create > Prefab** puis lui donner le nom du nouveau bâtiment.

Une fois le prefab créé, entrer dans sa fenêtre d'édition en double cliquant dessus, vous avez maintenant accès au prefab.  
Il suffit maintenant de lui indiquer qu'il est un bâtiment, pour ce faire on va lui ajouter un nouveau composant :  
**Add Component > Building**

Le composant *Building* vous permet de configurer les différents coûts de ce bâtiment.

Pour ajouter un bouton créant ce nouveau bâtiment, il faut se rendre dans l'objet *Canvas* et y créer un nouveau bouton :
**Click droit sur Canvas > UI > Button** et nommer ce bouton.

Le texte du bouton est contenu en enfant du bouton créé, quand à lui le bouton contient un composant *Button*.
Ce composant *Button* contient le champs *On Click ()* qui nous intéresse.  
Clicker sur le **+** pour ajouter un nouvel appel, laisser *Runtime Only*, l'objet à appeler est l'*Input Manager* de la **Scene** et la fonction est :  
**InputManager > Construct (GameObject)**

Un nouveau champs est apparu contenant le *GameObject* à passer en paramètre, il faut donc sélectionner le bâtiment créé dans l'onglet **Assets**.

Bravo, vous avez créé un nouveau bâtiment !