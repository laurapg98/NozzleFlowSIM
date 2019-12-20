SIMULATOR
The simulation will consist of predict the behavior of a discretized in one dimension nozzle (the way we have explained in the equations section) when a flow circulates through it.
With the equations that have been already calculated and starting from an initial state, all the properties of a nozzle for a subsequent time can be calculated and consequently displayed graphically.
During this section, the program will be explained, focusing on the code, which will include how does it works and what is it able to do.
1.1	Functionalities
As the objective of the project is to create an application in which the user will be able to create different types of nozzle by changing the shape, the main functionalities our program shall have:
Parameter box in which the user will be able to choose the starting parameters: number of rectangles of the nozzle, the horizontal axis step and the courant parameter.
Graphic display showing the shape and values of the different parameters of the nozzle: temperature, density, pressure and speed. The user can choose which one of these parameters colors scales wants to see in the control box and can see the exact value of each one of the properties in one of the tabs.
Control box allowing the user to play and pause the simulation (also with steps: forward and backward), reset the simulation and change the type of parameters showed in the graphic display.
Tabs box, which include the visualization of different graphs showing the values of the nozzle parameters along time and the horizontal axis of the nozzle and a table showing the updated exact values of the mentioned parameters. Also it includes other plots related with the mass flow.
Possibility of saving in a .txt file the status of the nozzle and loading a previously saved simulation.
Comparison of the Anderson book “Computational Fluid Dynamics” values with the values obtained from our simulation (with the same parameters) to verify the simulator performance. It also shows a table with the average difference in percentage of each one of the discretization points.

1.2	Code organization
For the programming part we will be using WPF C# with Visual Studio 2019.
The project will be divided into two different libraries: one holding the classes which will calculate the equations; and the other one with the display library, consisting of the WPF windows which will display and manage the informations and methods of the class library.
		1.2.1	Classes library 
This library contains two classes: one describing a discretized point of the nozzle and another describing the whole nozzle, which means a set of points.
Rectangulo class: Represents each dx inside the nozzle. Holds all the parameters calculated from the equations, which also holds. Each parameter is represented in an attribute and all of them have a present and a future value, for the calculation of each cycle.

1.2.1.1. Rectangulo class attributes
This class contains some different constructors and methods, that help us to optimize the execution of the code. Regarding to the constructors, one of them is empty, which means that creates a Rectangulo without any attribute; there is a copy constructor, which receives another Rectangulo and copy all the attributes; and finally two more constructors that receive as inputs the properties of the nozzle flow and assign it in the attributes. The difference between these two attributes is the fact that one of these has as input only the properties of the fluid (temperature, density, speed and pressure) and the other one also has the properties of the nozzle (height and area, which are related). Regarding to the methods, it has the typical ones and three more that compute each one of the parameters. 

One one hand, these mentioned typical ones are the setters and getters, that are used to get and set each one of the attributes of the class. On the other hand, the method related with the equations are separated into three: the first one compute the predicted future value using MacCormack’s technique, the second one corrects these previously computed predicted value using the corrector-predictor method, and the last one finally changes the present values with the obtained future values.

1.2.1.2. MacCormack’s technique applied in the code

1.2.1.3. Predictor-Corrector method applied in the code

Nozzle class: Represents the mesh in which the Rectangulo class objects are stored and how they are related to one another. Gives them an order and a height based on the equation of the nozzle shape.
By creating a nozzle we automatically create a vector of Rectangulos with a certain number of Rectangulo, each one of them with certain height and fluid properties, as we have explained in the equations section. The attributes of this class are the vector and the number of Rectangulos and a parameter needed in the advanced study, which will be used to situate the throat.

1.2.1.4. Nozzle class attributes
This class Nozzle has also setters, getters (for the number of rectangles, the rectangles vector and for one rectangle in a given position) and other some constructors and other methods. On one hand, the constructors are one empty, one that copy another Nozzle, and two that creates the nozzle: one with the throat in the middle (symmetrical nozzle with respect to the horizontal axis) and another one with the throat in a given position  (for the advanced study). One the other hand, the methods carry out some tasks that will be needed during the simulation, such as a function that execute one cycle, one that computes the outflow boundary conditions, a function that update the present state (changes from the one set in future), two functions that save or read the state to or from a .txt file, another one that gives a table with all the data of the nozzle in the current state and some other that returns list of the properties.

1.2.1.5. Cycle execution applied in the code
		1.2.2	Graphic library
Then we have the graphic library or, as we named it, the Nozzle Display. This is a library which contains the WPF windows that will use the class library to perform the simulation and show it on screen by using graphic libraries, plots, tables and similar tools. 
Each one of these windows will be explained in the next sections below.

1.3	Results presentation
The results of our simulator will be presented on the main window of the application, which will be divided into to four different zones, one for the input of the decided user values for the simulation, another for the control of the simulation (play, pause, reset...), one for the graphic display of the nozzle during the simulation (color scale in a graphical representation) and the last zone which is a Tab Control with four different tabs.
Parameter input
TextBoxes for different simulation parameters: the Courant parameter, the number of divisions of the nozzle and horizontal axis step. As figure 3.3.1. shows, there are two different buttons: the ‘Default’ writes the parameters that the Anderson book set: the Courant parameter equal to 0.5 and the nozzle formed by 30 rectangles of 0.1 width. The other one reads the values of the TextBoxes and builds the nozzle.

1.3.1. Main parameters
to start the simulation
Graphic nozzle 
Display of the nozzle graphically with a different color scale for each of the four main parameters being updated which each cycle

1.3.2. Display of each one of the nozzle parameters
Simulation control 
Buttons for the play and pause, step (backward or forward) and reset of the simulation. Also a ComboBox for the selection of which parameter the user wants to show in the display.

1.3.3. Control buttons
Tab Control
Different tabs which allow the user to navigate between them. Contains the different plots showing the values of the mentioned parameters along the length of the nozzle, the evolution of the parameters during time, values of the parameters on the nozzle during different steps and the updated values of the simulation of every parameter in the form of a Data Grid (similar to how the tables from Anderson are shown).

1.3.4. Different tab options
Upper menu
There is also a menu in the upper left side of the main window with four items. The first one of them (‘File’) allows the user of saving the current status of the nozzle in a .txt file and be able to open it whenever he/she wants to do it in order to go on with the simulation. The ‘Anderson’ one shows the table of results that the Anderson book gives and the table of results obtained from our simulation, in order to check the validity of the code. The next ones are extra: one in order to help the user if he/she does not understand which will include a video explanation of how does the application work, another one with the members of the group that have developed the application and the last one in order to open a .pdf report, which helps the user to understand the code.

1.3.5. Upper menu options

1.4	Application forms
As we have mentioned, the graphic library of the application has four forms, each one of them accomplishing a different task while running the code.
Main Window: The window in charge of the display of the results and data, control of the simulation and where the access to the other windows will be located, or in other words, the parent window.  It is the window that we have been describing in the previous section.
Save/Load Window: The window in which it will be possible to save the current state of the nozzle and load a previously saved state to go on with the simulation.
Anderson Window: This window will allow us to check the validity of our results by comparing them with the ones given by Anderson book in the easiest way possible, using WPF DataGrid templates. It is composed by three different tables: one with the data of the book, another one with the values from out program simulation and the last one with the average difference (in percentage) between the values of each discretization point.
Help Window: Simple window explaining each of the components of the simulation and how does the application work in general, in case of doubt while execution.
About us Window: Window with the team workers of the development of this project.
