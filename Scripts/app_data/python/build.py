import json
from os import error


def build_models(file_name):
    """Compile COBOL file and build models."""
    global program_model, identification_model, environment_model, data_model, procedure_model
    program_model = compile_cobol(file_name)
    identification_model = program_model.get_identification_division()
    environment_model = program_model.get_environment_division()
    data_model = program_model.get_data_division()
    procedure_model = program_model.get_procedure_division()
    return identification_model, environment_model, data_model, procedure_model

