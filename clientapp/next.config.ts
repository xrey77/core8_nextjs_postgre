import type { NextConfig } from "next";

// const nextConfig: NextConfig = {
//   /* config options here */
// };

// export default nextConfig;

    // next.config.js
    /** @type {import('next').NextConfig} */
    const nextConfig = {
      images: {
        remotePatterns: [
          {
            protocol: 'https',
            hostname: 'localhost', // Replace with the actual hostname of your image URL
            port: '7292',
            pathname: '/users/**', // Optional: specify a path
          },
        ],
      },
    };

    module.exports = nextConfig;