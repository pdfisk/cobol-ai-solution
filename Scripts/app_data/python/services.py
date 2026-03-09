def compile_cobol(file_name):
    """Compile a cobol file and return its model."""
    return compiler_service.compile_cobol_file(file_name)


def compile_and_save_cobol(file_name):
    """Compile a cobol file and save its model."""
    return compiler_service.compile_and_save_cobol_file(file_name)


def load_model(file_name):
    """Load a model from a JSON file."""
    model = data_service.get_model(file_name)
    return model
