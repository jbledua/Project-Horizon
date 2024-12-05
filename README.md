# Project Horizon
![Horizon-Logo](Horizon-Background.png)

**Project Horizon** is a Unity-based multiplayer aerial combat game where players pilot jets to engage in thrilling dogfights over an infinite, procedurally generated terrain. The game combines the traditional Unity GameObject structure for map generation with ECS (Entity Component System) + DOTS (Data-Oriented Technology Stack) for optimized player input and multiplayer networking.

## Features

- **Infinite Terrain Generation:** Procedurally generated environments using GameObject-based systems inspired by traditional Unity workflows.
- **ECS + DOTS Optimization:** Smooth and efficient player movement and networking powered by Unity's ECS and DOTS.
- **Multiplayer Gameplay:** Host public or private games, or join a dedicated server for an engaging multiplayer experience.
- **Single-Player Mode:** Explore the terrain and practice your piloting skills in a private game.

## Repositories

- **Main Project Repository:** [Project Horizon](https://github.com/jbledua/Project-Horizon) (This Repository)
- **Build Repository for Testing:** [Project Horizon Builds](https://github.com/jbledua/Project-Horizon-Build)

## Learning Resources

This project was inspired and built upon ideas from the following creators:

- **Map Generation:** Check out Sebastian Lague's excellent tutorials on [Procedural Map Generation](https://www.youtube.com/watch?v=MRNFcywkUSA&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=1).
- **ECS Player Input:** Learn ECS fundamentals and player input integration from Turbo Makes Games’ [ECS Tutorials](https://youtu.be/bFHvgqLUDbE?si=vnmw9A5UZ38EjFF2).

## Build Instructions

To build and run **Project Horizon**, follow these steps:

### Requirements

- **Unity Version:** Unity 6000.0.28f1 or newer (URP-compatible).
- **Development Environment:** Git, Visual Studio, and Unity Hub.

### Steps

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/jbledua/Project-Horizon.git
   ```
2. **Open the Project in Unity:**
   - Launch Unity Hub.
   - Click `Add` and select the `Project-Horizon` folder.
3. **Set Build Target:**
   - Go to `File > Build Profiles`.
   - Select your platform 
        - Client Build (Windows, macOS, Linux)
        - Server Builds (Windows Server, macOS Server, Linux Server)
   - Click `Switch Platform`.
4. **Build the Game:**
   - Click `Build`.
        - For client builds you will have options to:
                - Create a **Private** game.
                - **Join** Muliplayer games where you specify Host *IP Address* and *Port Number*
                - **Host** muliplayer games where you specify the listening *Port Number* for others to join
        - For server builds:
                - It will automatically **host** on *Port 7979* but will not create the client interface
5. **Run the Game:**
   - **Hosting Options:**
     - Launch the client build and host a game directly from your instance. This option includes a client interface for gameplay while acting as the host.
     - Alternatively, start the server build to run a dedicated hosting instance. Share the server's public IP address (or Tailscale/VPN address) and port with players.
     - For advanced hosting, follow the **Optional AWS EC2 Deployment** steps to run the server in a cloud environment.
   - **Joining a Game:**
     - Use the client build to connect to a hosted game by entering the host's IP address and port.
     - If hosting locally, you can connect to the server instance on the same machine or network by using `localhost` or the local IP address.


### Optional: Hosting the Server on AWS EC2

1. **Launch an EC2 Instance:**
   - Choose an Ubuntu or Windows Server AMI based on your platform needs.
   - Select an instance type suitable for hosting (e.g., `t2.medium` or higher is recommended).
   - Configure security group settings (see step 4).

2. **Upload the Server Build:**
   - Use an SCP client (e.g., `scp` or FileZilla) to transfer the server build to your EC2 instance.
   - Or you can clone the [Build Repo](https://github.com/jbledua/Project-Horizon-Build) to your EC2 instance.

3. **Run the Server:**
   - SSH into your instance and navigate to the uploaded server directory.
   - Ensure the server binary has executable permissions. If not, make it executable by running:
     ```bash
     chmod +x ProjectHorizonServer.x86_64
     ```
   - Execute the server binary:
     ```bash
     ./ProjectHorizonServer.x86_64
     ```

4. **Open a Port for Multiplayer:**
   - In the AWS Management Console, go to the **Security Groups** section.
   - Edit the **Inbound Rules** to allow traffic on **Port 7979** (or your specified port):
     - Protocol: **TCP**
     - Port Range: **7979**
     - Source: **0.0.0.0/0** (for public access) or your specific IP range.

5. **Connect to the Server:**
   - Share your EC2 instance's public IP address with players.
   - Players can join by entering the IP address and port in the game client.

#### EC2 Notes

- Make sure to terminate the EC2 instance or pause hosting when not in use to avoid unnecessary charges.
- Adjust the security group rules carefully to ensure your server is secure while accessible.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

Thanks to the Unity community and the creators of ECS and map generation tutorials for their incredible resources and inspiration.