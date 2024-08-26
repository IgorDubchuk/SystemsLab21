# SystemsLab21
In my solution, I tried to adhere to the principles of clean architecture and clean code as much as possible. Not all methods were able to be made short enough, but such was their logic.

I started the design by modeling the domain, forming domain entities, and their invariants. I tried to make the domain model as rich as I could. I believe that in solutions where the cost of error can be high, it is better to spend extra time and lay everything out on the shelves.

Thus, I tried to isolate the domain logic from the application layer and API logic. I considered that in a real application, such a service would most likely have some kind of persistent data storage, and such data as the station topology and available paths, in reality, may not be request parameters, but stored in a database. Therefore, I used the dependency inversion principle, so that interfaces for accessing saved data are declared at the domain model level, and implemented at the infrastructure level.

But in our application, all data comes from the request. Therefore, I decided to create one-time repositories that implement the required interfaces, and the first thing after receiving the request is to save the received data in the repository in the way that, in my opinion, it would happen in real life: first, the topology of paths is configured: all nodes and all connections between them, then, in accordance with the topology, a set of allowed paths is created.

When all the data is prepared, we can run the domain logic of determining the presence of a safe path. I considered a safe path to be one that is initially marked as free, and all of whose nodes are themselves free, that is, they are not part of any other busy path.
The logic of filling repositories is controlled by the application layer in the form of a MediatR handler. It is there that the primary request validation is performed, that is, checks that are not limitations of domain entities, but are rather a consequence of the chosen contract and the use case being implemented at the application level.

The contract, by the way, was slightly changed to eliminate the ambiguity of the path definition. I believe that it is correct to set the path not by the start and end points, but by a complete enumeration of the topology nodes in the order in which they occur in this path.
I deployed my application to Azure App Service, where you can familiarize yourself with it.

Swagger:
https://systemlabs21-ftfsb5cpdzaefcbg.eastus-01.azurewebsites.net/swagger/index.html
