// components/JQueryComponent.tsx
"use client"; // Must be at the very top for Next.js 15

import { useEffect } from 'react';
import $ from 'jquery';

const JQueryComponent = () => {
  useEffect(() => {
    // jQuery code here runs only in the browser
    $('.my-element').css('color', 'blue');
  }, []);

  return null; // Or your JSX
};

export default JQueryComponent;
