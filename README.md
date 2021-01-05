# Okolni-SourceEngine-Ouery
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![LGPL License][license-shield]][license-url]
[![Nuget][nuget-shield]][nuget-url]
<!-- [![LinkedIn][linkedin-shield]][linkedin-url] -->



<p align="center">
<br />
<a href="https://github.com/Florian2406/Okolni-Source-Query/blob/master/doc/Okolni.Source.Example/Program.cs">View Demo</a>
·
<a href="https://github.com/Florian2406/Okolni-Source-Ouery/issues">Report Bug</a>
·
<a href="https://github.com/Florian2406/Okolni-Source-Ouery/issues">Request Feature</a>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

The project is a simple package made for C#/.NET Source Query Requests. Main features are the default requests that can also be found on the source query documentation in the [Valve developer community wiki](https://developer.valvesoftware.com/wiki/Server_queries). For a list of implemented requests see the roadmap below.

### Built With

* [.NET-Standard 2.0](https://docs.microsoft.com/de-de/dotnet/standard/net-standard)
* [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)

## Usage

First download the [nuget package](https://www.nuget.org/packages/Okolni.Source.Query/) and import the namespace `Okolni.Source.Query` which is the main namespace. After that you can use the project as in the code example below. You create a query connection with an ip and a port and after connecting you can get started with your requests.
```
IQueryConnection conn = new QueryConnection();

conn.Host = "127.0.0.1"; // IP
conn.Port = 27015; // Port

conn.Connect(); // Create the initial connection

var info = conn.GetInfo(); // Get the Server info
var players = conn.GetPlayers(); // Get the Player info
var rules = conn.GetRules(); // Get the Rules
```
_For an example view the demo project [Demo](https://github.com/Florian2406/Okolni-Source-Query/blob/master/doc/Okolni.Source.Example/Program.cs)_

<!-- _For more examples, please refer to the [Documentation](https://example.com)_ -->


## Roadmap

Implemented so far:
- Source Query Servers
- Info Request
- Players Request
- Rules Request
- The Ship Servers

Missing at the moment:
- Multipacket responses
- Goldsource Servers

Also see the [open issues](https://github.com/Florian2406/Okolni-Source-Ouery/issues) for a list of proposed features (and known issues).


## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


## License

Distributed under the LGPL-3.0 License. See `LICENSE` for more information.


## Contact

Florian Adler - [@florian2406](https://twitter.com/florian2406)

Project Link: [https://github.com/Florian2406/Okolni-Source-Ouery](https://github.com/Florian2406/Okolni-Source-Ouery)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [GitHub Emoji Cheat Sheet](https://www.webpagefx.com/tools/emoji-cheat-sheet)
* [Img Shields](https://shields.io)
* [Choose an Open Source License](https://choosealicense.com)
* [Valve developer community](https://developer.valvesoftware.com/wiki/Server_queries)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/florian2406/Okolni-Source-Query?style=for-the-badge
[contributors-url]: https://github.com/florian2406/Okolni-Source-Query/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/florian2406/Okolni-Source-Query?style=for-the-badge
[forks-url]: https://github.com/florian2406/Okolni-Source-Query/network/members
[stars-shield]: https://img.shields.io/github/stars/florian2406/Okolni-Source-Query?style=for-the-badge
[stars-url]: https://github.com/florian2406/Okolni-Source-Query/stargazers
[issues-shield]: https://img.shields.io/github/issues/florian2406/Okolni-Source-Query?style=for-the-badge
[issues-url]: https://github.com/florian2406/Okolni-Source-Query/issues
[license-shield]: https://img.shields.io/github/license/florian2406/Okolni-Source-Query?style=for-the-badge
[license-url]: https://github.com/florian2406/Okolni-Source-Query/blob/master/LICENSE
[nuget-shield]: https://img.shields.io/nuget/dt/Okolni.Source.Query?style=for-the-badge
[nuget-url]: https://www.nuget.org/packages/Okolni.Source.Query