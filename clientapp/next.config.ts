import type { NextConfig } from "next";
// const path = require('path');
import path from 'path';
// const nextConfig: NextConfig = {
//   /* config options here */
// };

// export default nextConfig;

    // next.config.js
    /** @type {import('next').NextConfig} */
    const nextConfig = {
      outputFileTracingRoot: path.join(__dirname, '../../'),
      images: {
        remotePatterns: [
          {
            protocol: 'https',
            hostname: 'localhost', 
            port: '7292',
            pathname: '/**'
          },
          {
            protocol: 'https',
            hostname: '127.0.0.1', 
            port: '7292',
            pathname: '/**'
          },
        ],
      },
    };

    module.exports = nextConfig;