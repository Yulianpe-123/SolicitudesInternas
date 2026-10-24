# Guion de demostración – Sistema de Seguimiento de Solicitudes Internas

Duración sugerida: 10–12 minutos. Antes de empezar: ejecutar el script SQL (datos de ejemplo limpios) y abrir la aplicación con F5.

## 0. Presentación (1 min)
- Mostrar el **menú principal**: tarjetas con el conteo por estado.
- Explicar en una frase la arquitectura: View → Controller → Service → Repository → Base de datos, con interfaces entre capas.

## 1. Ingreso de información (2 min)
1. **Categorías → Nueva categoría**: crear "Software" (descripción libre). Se muestra el mensaje de confirmación.
2. **Usuarios → Nuevo usuario**: registrar un usuario nuevo.
3. **Responsables → Nuevo responsable**: registrar uno nuevo.
4. **Nueva solicitud**: solicitante "Carlos Méndez", categoría "Software", prioridad Alta, descripción de más de 10 caracteres. Se redirige al detalle (#6) con estado **Creada** y un primer registro en el historial.

## 2. Consultas (1 min)
- **Solicitudes**: filtrar por **responsable** (Sofía Díaz), luego por **estado** (En proceso) y luego por **prioridad** (Crítica). Mostrar "Limpiar".
- **Responsables → "Sus solicitudes"**: acceso directo a las solicitudes de un responsable.

## 3. Proceso completo con la solicitud #6 (3 min)
1. **Asignar** a Jorge Herrera → pasa a **Asignada**; el historial registra la asignación.
2. **Cambiar prioridad** a Crítica con un motivo → queda registrado en el historial.
3. **Registrar seguimiento** (autor "Jorge Herrera", un comentario).
4. **Reasignar** a Luis Morales → el historial muestra "reasignada de Jorge Herrera a Luis Morales".
5. **Iniciar atención** → **En proceso**.
6. **Resolver** con una solución → **Resuelta**.
7. **Cerrar** → **Cerrada**. Mostrar que desaparece el panel de operaciones y aparece el aviso de solo lectura.

## 4. Reglas de negocio y manejo de errores (3 min)
Cada intento muestra un mensaje comprensible (sin errores técnicos):

| Prueba | Resultado esperado | Regla |
|---|---|---|
| En la solicitud #1 (Creada) pulsar **Cerrar** | "No se puede cerrar… el responsable debe registrar primero una solución." | RN-08 |
| En la solicitud #1 pulsar **Iniciar atención** | "No es posible pasar de 'Creada' a 'En proceso'. El siguiente paso permitido es 'Asignada'." | RN-02 |
| En la solicitud #2 (Asignada) pulsar **Resolver** con una solución | "No es posible pasar de 'Asignada' a 'Resuelta'. El siguiente paso permitido es 'En proceso'." | RN-02 |
| En la solicitud #3 (En proceso) pulsar **Resolver** con la solución vacía | Mensaje de validación de entrada (solución obligatoria) | Validación |
| En la solicitud #3, **Resolver** con "ok" | "Para resolver la solicitud debe registrar una solución de al menos 10 caracteres." | RN-05 |
| En la solicitud #2 reasignar al **mismo** responsable | "La solicitud ya está asignada a… Seleccione un responsable distinto." | RN-04 |
| En la solicitud #2 pulsar **Asignar** sin elegir responsable | "Debe seleccionar el responsable." | RN-04 |
| Asignar a **Raúl Ortiz** (inactivo, no aparece en la lista; probar editando el combo o desde otro responsable) | Responsable inactivo no puede recibir solicitudes | RN-09 |
| **Responsables → Editar** a Sofía Díaz, desmarcar *Activo* | "No se puede desactivar a Sofía Díaz porque tiene N solicitud(es) sin cerrar." | RN-09 |
| Abrir la solicitud #5 (Cerrada) | Sin operaciones; aviso de solo lectura | RN-03 |
| **Nueva solicitud** con descripción "hola" | Validación: la descripción debe tener entre 10 y 1000 caracteres | RN-01 |
| **Nueva solicitud** sin elegir solicitante | "Debe seleccionar el solicitante." | RN-01 |
| **Usuarios → Nuevo** con un correo ya existente | "Ya existe un usuario registrado con el correo…" | RN-10 |
| Ir a `/Solicitudes/Detalle/999` | Pantalla "La solicitud #999 no existe." | Registro inexistente |

## 5. Cierre (1 min)
- Mostrar el historial completo de la solicitud #6 (todas las acciones, en orden, con autor y fecha).
- Mencionar que la base de datos refuerza reglas con restricciones `CHECK` y que no existe ninguna opción para eliminar solicitudes ni seguimientos.
