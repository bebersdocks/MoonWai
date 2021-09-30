ELMISH_DIR = MoonWai.Elmish
ELMISH_OUTPUT_DIR = MoonWai.Elmish/public
ELMISH_TARGET_DIR = MoonWai.Api/wwwroot/dist

clean_elm:
	rm $(ELMISH_OUTPUT_DIR)/bundle.js -f
	rm $(ELMISH_TARGET_DIR) -rf

build_elmish:
	make clean_elm
	cd $(ELMISH_DIR); npm run build
	mkdir $(ELMISH_TARGET_DIR) -p
	cp $(ELMISH_OUTPUT_DIR)/* $(ELMISH_TARGET_DIR)