ELM_DIR = MoonWai.Elm
ELM_MAKE_OUTPUT = index.html
ELM_TARGET_DIR = MoonWai.Api/Resources/Elm

clean_elm:
	rm $(ELM_DIR)/$(ELM_MAKE_OUTPUT) -f
	rm $(ELM_TARGET_DIR) -rf

build_elm:
	make clean_elm
	cd $(ELM_DIR); elm make src/Main.elm --output=$(ELM_MAKE_OUTPUT)
	mkdir $(ELM_TARGET_DIR)
	mv $(ELM_DIR)/$(ELM_MAKE_OUTPUT) $(ELM_TARGET_DIR)/$(ELM_MAKE_OUTPUT)

build:
	make build_elm
	dotnet build

run:
	make build_elm
	dotnet run
