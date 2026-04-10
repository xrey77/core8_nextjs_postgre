'use client'; // This directive marks the file as a Client Component

import { useEffect } from 'react';
declare module 'bootstrap/dist/js/bootstrap.bundle.js';

export default function BootstrapClient() {
  useEffect(() => {
    
    import("bootstrap/dist/js/bootstrap.bundle.js");
  }, []); // The empty dependency array ensures this runs once, on mount

  return null; // This component doesn't render anything itself
}
