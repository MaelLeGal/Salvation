# Salvation - Partie Niko

## Syst�me de Tick

Le syst�me de tick (*Ticking System*) est le syst�me temporel utilis� par le jeu.
Le syst�me est modulable, c'est � dire qu'on peut d�finir le nombre de secondes entre chaque tick pour acc�l�rer ou ralentir le jeu.

## Placement des b�timents

Un b�timent contient 3 bool�ens d�finissant s'il peut ou non �tre pros� sur un certain type de case :
- Pla�able sur l'herbe (*PlaceableOnGrass*) : le b�timent peut �tre construit sur l'herbe (le terrain alli�).
- Pla�able sur terrain neutre (*PlaceableOnNeutral*) : le b�timent peut �tre construit sur terrain neutre.
- Pla�able sur terrain corrompu (*PlaceableOnDryGround*) : le b�timent peut �tre construit sur terrain corrompu (le terrain ennemi).

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

Le composant *Building* vous permet de configurer les diff�rents param�tres de ce b�timent.

Pour ajouter un bouton cr�ant ce nouveau b�timent, il faut se rendre dans l'objet *Canvas* et y cr�er un nouveau bouton :
**Click droit sur Canvas > UI > Button** et nommer ce bouton.

Le texte du bouton est contenu en enfant du bouton cr��, quand � lui le bouton contient un composant *Button*.
Ce composant *Button* contient le champs *On Click ()* qui nous int�resse.  
Clicker sur le **+** pour ajouter un nouvel appel, laisser *Runtime Only*, l'objet � appeler est l'*Input Manager* de la **Scene** et la fonction est :  
**InputManager > Construct (GameObject)**

Un nouveau champs est apparu contenant le *GameObject* � passer en param�tre, il faut donc s�lectionner le b�timent cr�� dans l'onglet **Assets**.

Bravo, vous avez cr�� un nouveau b�timent !

## Ev�nement de cr�ation de b�timents sp�ciaux

Certains b�timents sont sp�ciaux, au lieu de consommer/produire des ressources par tick ils vont uniquement d�clencher un �v�nement a leur construction.

Exemple : on consid�re le b�timent *Puit*, au moment de la cr�ation du *Puit* les cases adjacentes vont devenir de l'herbe.

Pour ce faire j'ai utilis� le syst�me d'�v�nements int�gr� dans C#.  
En effet le singleton *InputManager* d�clenche l'�v�nement *OnConstruct* lors de la cr�ation d'un b�timent avec comme argument une structure *ConstructEventArgs*.  

### ConstructEventArgs (WIP)

Cette structure contient diff�rentes informations importante pour l'�v�nement :
- Type (*Type*) : d�fini ce qu'il se passe lors de la cr�ation d'un b�timent.
  - Aucun (*None*) : la construction n'aura aucun effet particulier.
  - Herbe (*Grass*) : change des cases en herbe.
  - Terrain neutre (*Neutral*) : change des cases en terrain neutre.
  - Terrain corrompu (*DryGround*) : change des cases en terrain corrompu.
- Pattern (*Pattern*) : d�fini le pattern de changement des cases. Il est identifi� par un Vector2 d�finissant la zone d'effet.
  - Exemple : [1; 3] =>

    | O | X | O |
    |---|---|---|
    | **O** | *X* | **O** |
    | **O** | **X** | **O** |

  - Exemple : [3; 1] =>

    | O | O | O |
    |---|---|---|
    | **X** | *X* | **X** |
    | **O** | **O** | **O** |

  - Exemple : [3; 3] =>

    | X | X | X |
    |---|---|---|
    | **X** | *X* | **X** |
    | **X** | **X** | **X** |

- Case (*Tile*) : r�f�rence la case sur laquelle le b�timent est construit (affect�e par l'*InputManager*).