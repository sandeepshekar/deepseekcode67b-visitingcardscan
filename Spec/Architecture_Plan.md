# Architecture Plan: Local Media Viewer (.NET C#)

**Project Goal:** To create a simple, polished, yet highly effective offline Windows desktop application using C# .NET that views local photos and videos from a user-selected folder.

## 1. Technology Stack Recommendation & Justification

*   **Framework:** **WinUI 3 (or WPF as fallback)**
    *   **Justification:** WinUI 3 offers the modern, native look and feel required for a "polished" Windows experience. It is optimized for current Windows versions. If implementation complexity with bleeding-edge libraries becomes an issue, WPF provides a highly stable and battle-tested alternative within the .NET ecosystem.
*   **Language:** C# (.NET 8 or later)

**Key Libraries (NuGet Packages):**
*   **Image Handling:** **SixLabors.ImageSharp:** Recommended for reading and manipulating various image formats (JPG, PNG) robustly, avoiding dependency on legacy GDI+ limitations.
*   **Video Playback:** **VLC.NET:** Best practice for handling diverse video codecs (MP4, MOV) reliably in a desktop application without deep native system codec configuration issues.
*   **Concurrency:** Standard C# `Task` and `async`/`await` patterns must be used extensively throughout the stack to ensure all I/O operations are non-blocking, guaranteeing UI responsiveness.

## 2. Architectural Pattern & Data Flow (MVVM)

We will strictly follow the Model-View-ViewModel (MVVM) pattern.

**Data Flow:**
1.  **File System Service $\rightarrow$ Model:** The `IFileScanService` reads local files and populates raw **Model** objects (`MediaFile`).
2.  **Services $\rightarrow$ ViewModel:** ViewModels consume the `MediaFile` list, calling specialized services (e.g., `ITumbnailService`) to process paths into ready-to-display data structures that are optimized for presentation.
3.  **ViewModel $\rightarrow$ View:** The ViewModel exposes an `ObservableCollection<T>` of view models (`GalleryViewModel`), which the **View** binds against, allowing automatic UI updates when data changes (e.g., new thumbnail loaded).

## 3. Project Structure

The structure must enforce clean separation:

*   `Models/`: Core data representation (e.g., `MediaFile.cs`).
*   `ViewModels/`: Presentation logic and state management (e.g., `MainViewModel.cs`, `GalleryViewModel.cs`).
*   `Views/`: UI definitions (XAML). Must be lightweight, containing minimal code-behind.
*   `Services/`: All infrastructure logic. This layer handles external concerns:
    *   `IFileScanService.cs`: Folder traversal and metadata gathering.
    *   `ITumbnailService.cs`: Thumbnail generation and caching logic (Crucial for performance).
    *   `IMediaService.cs`: Wrapper coordinating loading between `ImageSharp` and VLC.NET.

## 4. Implementation Roadmap (Phased Approach)

**Phase 1: Setup and File Discovery (The Backbone)**
1.  Setup the WinUI/WPF project structure.
2.  Implement `IFileScanService`: Scan a given directory, filter media extensions (`*.jpg`, `*.jpeg`, `*.png`, `*.mp4`), and populate basic model data quickly.
3.  Connect the main View to receive the initial file list via the ViewModel.

**Phase 2: Gallery View (The Thumbnail Grid)**
1.  Refine `ITumbnailService`: Implement a robust, in-memory caching layer for generated thumbnails. This prevents redundant CPU usage when multiple items are displayed or scrolled past.
2.  Implement View virtualization (`ItemsControl` with `VirtualizingPanel`) to handle hundreds of files without memory spikes.
3.  Bind the Gallery View using this service's cached results, ensuring thumbnail loading is explicitly asynchronous per item.

**Phase 3: Single Media Viewer (The Core Feature)**
1.  Implement dynamic component switching within a single control.
2.  When an item is selected in Phase 2, call `IMediaService` to determine type:
    *   If Image: Load and display the full-resolution image bitmap using `ImageSharp`.
    *   If Video: Initialize and play the video file path using the VLC component wrapper.

## Summary & Next Steps
This plan provides a high-performance, modern foundation for the application. We should proceed by starting with **Phase 1** to establish the foundational ability to scan files correctly.