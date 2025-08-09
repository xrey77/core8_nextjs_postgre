import Head from 'next/head'
import styles from '../styles/Home.module.css'
import Image from 'next/image'
import car1 from '../public/images/1.jpeg';
import car2 from '../public/images/2.jpeg';
import car3 from '../public/images/3.jpeg';
import car4 from '../public/images/4.jpeg';

// import { faArrowRight } from '@fortawesome/free-solid-svg-icons';

export default function Home({}) {
  return (
    <>
      <main className="container-fluid bg-dark">
      <div id="carouselExampleIndicators" className="carousel slide">
        <div className="carousel-indicators">
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
        </div>
        <div className="carousel-inner">
          <div className="carousel-item active">
            <Image src={car1} className="d-block img" alt="..."/>
          </div>
          <div className="carousel-item">
            <Image src={car2} className="d-block img" alt="..."/>
          </div>
          <div className="carousel-item">
            <Image src={car3} className="d-block img" alt="..."/>
          </div>
        </div>
        <button className="carousel-control-prev" type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide="prev">
          <span className="carousel-control-prev-icon" aria-hidden="true"></span>
          {/* <span className="visually-hidden">Previous</span> */}
        </button>
        <button className="carousel-control-next" type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide="next">
          <span className="carousel-control-next-icon" aria-hidden="true"></span>
          
          <span className="visually-hidden"></span>
        </button>
      </div>
      </main>
    </>
  )
}
