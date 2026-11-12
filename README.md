# FitArmLog

[![CI](https://github.com/pasilisuces-netizen/FitArmLog/actions/workflows/ci.yml/badge.svg)](https://github.com/pasilisuces-netizen/FitArmLog/actions/workflows/ci.yml)

Aplicación móvil **.NET MAUI** para registrar entrenamientos de gimnasio: armás rutinas con ejercicios, registrás series (repeticiones y peso), y guardás cada sesión con foto, ubicación y notas.

Proyecto de la materia *Desarrollo de Aplicaciones Móviles II* (UCES, Tecnicatura en Programación). Es la evolución de la app del Parcial 1.

## Funcionalidades
- Catálogo de ejercicios consumido desde una API pública (Free Exercise DB) con manejo de errores y timeout.
- Rutinas: crear, listar, ver detalle y eliminar.
- Entrenamiento: series con repeticiones y peso (validados), notas, **foto** con la cámara y **ubicación GPS** opcional.
- Historial persistente de entrenamientos.
- **Sensores**: sacudir el teléfono para agregar una serie (acelerómetro) y vibración como respuesta táctil.
- Ajustes: activar o desactivar vibración, sacudida y ubicación por defecto.
- Datos locales con **SQLite**.

## Arquitectura
```
View (XAML)  ->  ViewModel  ->  Service  ->  Repository (interfaz)  ->  SQLite
                                   |
                              AppValidator (validación centralizada)
```
- **MVVM** con CommunityToolkit.Mvvm (`[ObservableProperty]`, `[RelayCommand]`).
- **Capas**: las dependencias apuntan hacia adentro. Los ViewModels solo conocen servicios; los servicios solo conocen interfaces de repositorio.
- **Result&lt;T&gt;**: los servicios devuelven éxito o error sin lanzar excepciones hacia la UI.
- **Inyección de dependencias** en `MauiProgram.cs` (repositorios, servicios y servicios de dispositivo como Singleton; ViewModels y páginas como Transient).
- Servicios de dispositivo detrás de interfaces (`IPhotoService`, `ILocationService`, `IHapticService`, `IShakeDetector`) para poder reemplazarlos.

## Cómo ejecutar
Requisitos: Visual Studio 2022/2026 con la carga de trabajo **.NET MAUI** y .NET 9.

1. Clonar el repositorio y abrir `FitArmLog.sln`.
2. Elegir un emulador o dispositivo **Android** y ejecutar (F5).
   - La cámara, el GPS, el acelerómetro y la vibración se prueban mejor en un teléfono real.

## Tests
Proyecto `FitArmLog.Tests` (xUnit, .NET 9). Incluye:
- Pruebas unitarias del validador y los servicios (con **Fakes** y **Moq**).
- Pruebas de integración de los repositorios contra **SQLite en memoria**.
- Pruebas del consumo de API con un `HttpMessageHandler` simulado (sin internet).

```
dotnet test FitArmLog.Tests/FitArmLog.Tests.csproj
```

## CI/CD
- `.github/workflows/ci.yml`: en cada push o PR a `master` corre los tests (matriz Ubuntu y Windows, con caché de NuGet y resultados como artefacto) y, si pasan, compila el **APK Android** y lo guarda como artefacto.
- `.github/workflows/release.yml`: al crear un tag de versión (`v1.0.0`, SemVer) genera el APK y lo publica en **GitHub Releases**.

```
git tag v1.0.0
git push origin v1.0.0
```

## Permisos
| Permiso | Para qué | Tipo |
|---|---|---|
| `CAMERA` | Foto del entrenamiento | Peligroso (se pide en ejecución) |
| `ACCESS_FINE_LOCATION` / `ACCESS_COARSE_LOCATION` | Registrar dónde se entrenó (opcional) | Peligroso (se pide en ejecución) |
| `VIBRATE` | Respuesta táctil | Normal (se concede al instalar) |
| `INTERNET` / `ACCESS_NETWORK_STATE` | Catálogo de ejercicios | Normal |

Si el usuario deniega un permiso, la app sigue funcionando sin esa función (degradación elegante).

## Evolución desde el Parcial 1
| Parcial 1 | Ahora |
|---|---|
| Rutinas en memoria | Persistencia con SQLite |
| Lógica en los ViewModels | Capa de servicios + repositorios con interfaces |
| Validaciones dispersas | `AppValidator` centralizado |
| Sin pruebas | Pruebas unitarias e integración con xUnit, Moq y Fakes |
| Sin CI | GitHub Actions (tests + APK) y release por tag |
| Sin sensores | Cámara, GPS, acelerómetro y vibración con manejo de permisos |
