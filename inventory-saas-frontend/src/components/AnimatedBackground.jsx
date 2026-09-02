import { useRef, useMemo } from 'react';
import { Canvas, useFrame } from '@react-three/fiber';
import { Float, MeshDistortMaterial, Sphere, Torus, Box as DreiBox } from '@react-three/drei';
import { Box } from '@mui/material';

// Ek ghumta hua, glowing shape
function FloatingShape({ position, color, shape, scale = 1 }) {
  const meshRef = useRef();

  useFrame((state, delta) => {
    meshRef.current.rotation.x += delta * 0.15;
    meshRef.current.rotation.y += delta * 0.2;
  });

  return (
    <Float speed={1.5} rotationIntensity={0.5} floatIntensity={1.5}>
      <mesh ref={meshRef} position={position} scale={scale}>
        {shape === 'sphere' && <sphereGeometry args={[1, 64, 64]} />}
        {shape === 'torus' && <torusGeometry args={[1, 0.35, 32, 100]} />}
        {shape === 'box' && <boxGeometry args={[1.3, 1.3, 1.3]} />}
        <MeshDistortMaterial
          color={color}
          attach="material"
          distort={0.35}
          speed={1.5}
          roughness={0.15}
          metalness={0.9}
        />
      </mesh>
    </Float>
  );
}

function Scene() {
  return (
    <>
      <ambientLight intensity={0.4} />
      <pointLight position={[10, 10, 10]} intensity={1.2} color="#6366F1" />
      <pointLight position={[-10, -10, -10]} intensity={0.8} color="#EC4899" />

      <FloatingShape position={[-3.5, 1.5, 0]} color="#6366F1" shape="sphere" scale={1.1} />
      <FloatingShape position={[3.5, -1.5, -2]} color="#EC4899" shape="torus" scale={1} />
      <FloatingShape position={[2.5, 2.5, -3]} color="#818CF8" shape="box" scale={0.8} />
      <FloatingShape position={[-3, -2, -1]} color="#4F46E5" shape="sphere" scale={0.7} />
    </>
  );
}

function AnimatedBackground() {
  return (
    <Box
      sx={{
        position: 'fixed',
        top: 0,
        left: 0,
        width: '100%',
        height: '100%',
        zIndex: 0,
        background: 'radial-gradient(ellipse at center, #111827 0%, #0A0E17 100%)',
      }}
    >
      <Canvas camera={{ position: [0, 0, 8], fov: 45 }}>
        <Scene />
      </Canvas>
    </Box>
  );
}

export default AnimatedBackground;