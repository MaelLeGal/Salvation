# Salvation

## Système de Tick

Le système de tick (*Ticking System*) est le système temporel utilisé par le jeu.
Le système est modulable, c'est à dire qu'on peut définir le nombre de secondes entre chaque tick pour accélérer ou ralentir le jeu.

## Placement des bâtiments

Un bâtiment contient 3 booléens définissant s'il peut ou non être prosé sur un certain type de case :
- Plaçable sur l'herbe (*PlaceableOnGrass*) : le bâtiment peut être construit sur l'herbe (le terrain allié).
- Plaçable sur terrain neutre (*PlaceableOnNeutral*) : le bâtiment peut être construit sur terrain neutre.
- Plaçable sur terrain corrompu (*PlaceableOnDryGround*) : le bâtiment peut être construit sur terrain corrompu (le terrain ennemi).

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

### Le prefab

Pour créer un nouveau bâtiment il faut commencer par créer un nouveau prefab dans les assets des buildings.  
**Clic droit dans le dossier *Assets/Resources/Buildings* > Create > Prefab** puis lui donner le nom du nouveau bâtiment.

**⚠️ Ce nom doit être le même pour le prefab, le fichier JSON et la string passée au bouton !**

### Le JSON

Nous allons maintenant passer à la création du fichier JSON définissant ce bâtiment.  
Pour ce faire il s'agira de créer un objet vide dans la scène :  
**Create Empty**  
Puis on lui affecte le composant *JSONBuildingCreator* :  
**Add Component > JSONBuildingCreator**

Ce nouvel objet nous permet de configurer un building.
Une fois fait il suffit de cliquer sur le "bouton" *Save To JSON* qui créera le fichier JSON correspondant au bâtiment configuré dans le dossier *Assets/Resources/JSON/[Name]*.

**⚠️ Ce nom doit être le même pour le prefab, le fichier JSON et la string passée au bouton !**

Cet objet peut maintenant être supprimé de la scène, ou gardé pour créer d'autres bâtiments.
En effet cet objet ne sert à rien d'autre que de créer des JSON :).

### Le bouton

Pour ajouter un bouton créant ce nouveau bâtiment, il faut se rendre dans l'objet *Canvas* et y créer un nouveau bouton :
**Click droit sur Canvas > UI > Button** et nommer ce bouton.

Le texte du bouton est contenu en enfant du bouton créé, quand à lui le bouton contient un composant *Button*.
Ce composant *Button* contient le champs *On Click ()* qui nous intéresse.  
Clicker sur le **+** pour ajouter un nouvel appel, laisser *Runtime Only*, l'objet à appeler est l'*Input Manager* de la **Scene** et la fonction est :  
**InputManager > Construct (string)**

Un nouveau champs est apparu contenant la *string* à passer en paramètre, il faut donc entrer le nom du bâtiment.

**⚠️ Ce nom doit être le même pour le prefab, le fichier JSON et la string passée au bouton !**

Bravo, vous avez créé un nouveau bâtiment !

## Evènement de création de bâtiments spéciaux

Certains bâtiments sont spéciaux, au lieu de consommer/produire des ressources par tick ils vont uniquement déclencher un évènement a leur construction.

Exemple : on considère le bâtiment *Puit*, au moment de la création du *Puit* les cases adjacentes vont devenir de l'herbe.

Pour ce faire j'ai utilisé le système d'évènements intégré dans C#.  
En effet le singleton *InputManager* déclenche l'évènement *OnConstruct* lors de la création d'un bâtiment avec comme argument une structure *ConstructEventArgs*.  

### ConstructEventArgs

Cette structure contient différentes informations importante pour l'évènement :
- Type (*Type*) : défini ce qu'il se passe lors de la création d'un bâtiment.
  - Aucun (*None*) : la construction n'aura aucun effet particulier.
  - Herbe (*Grass*) : change des cases en herbe.
  - Terrain neutre (*Neutral*) : change des cases en terrain neutre.
  - Terrain corrompu (*DryGround*) : change des cases en terrain corrompu.
- Rayon (*Radius*) : défini le rayon de changement des cases.
- Position (*Position*) : référence la position sur laquelle le bâtiment est construit (affectée par l'*InputManager*).

## Pipeline (Bouton -> Création)

Au moment de la pression d'un bouton correspondant à un bâtiment :

**Bouton :**  
Le bouton pressé appelle la fonction *Construct* de l'*InputManager* avce comme argument le nom du bâtiment.

**InputManager :**  
La fonction *Construct()* commence par charger le JSON correspondant au bâtiment à créer.  
Il vérifie ensuite une série de conditions déterminant si le bâtiment est plaçable ou non.  
Si c'est le cas, le prefab correspondant au bâtiment est instantié.  
Il se charge aussi d'ajouter un composant *Building* au prefab en y chargeant les données du JSON.  
La case où est construite le bâtiment est déterminée comme occupée (*isOccupied = true*).  
Enfin la position de construction est ajoutée au *ConstructEventArgs* du bâtiment et celui-ci est propagé sous forme d'évènement.

**CorruptionManager :**  
Le *CorruptionManager* réceptionne l'évènement avec sa fonction *ConstructionEvent* et modifie le terrain comme indiqué dans le *ConstructEventArgs*.

**Fin**  
Le bâtiment réagit maintenant au jeu en exécutant ses productions et en payant ses coûts.