{
  description = "AutoPartsStore";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem
      (system:
        let
          pkgs = import nixpkgs { system = system; config.allowUnfree = true; };
        in
        {
          devShells.default = pkgs.mkShell {
            buildInputs = with pkgs; [
              dotnet-sdk
              dotnetPackages.Nuget
              unzip
              zip
              postman
              tree
	      wkhtmltopdf
	      google-chrome
	      ncpamixer
	      vscode
	      git-filter-repo
            ];
          };
        }
      );
}
