window.descargarArchivo = (base64String, nombreArchivo, tipoMine) => {
    const enlace = document.createElement('a');
    enlace.href = 'data:${tipoMine};base64,${base64String}';
    enlace.download = nombreArchivo;
    enlace.click();
};