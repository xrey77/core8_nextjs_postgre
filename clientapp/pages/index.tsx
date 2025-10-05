'use client'
import Image from 'next/image';
import Footer from './layout/footer';

export default function Home() {
  return (
    <>
      <main className="container-fluid bg-dark">
      <div id="carouselExampleIndicators" className="carousel slide">
        <div className="carousel-indicators">
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="1" aria-label="Slide 2"></button>
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="2" aria-label="Slide 3"></button>
          <button type="button" data-bs-target="#carouselExampleIndicators" data-bs-slide-to="3" aria-label="Slide 4"></button>
        </div>
        <div className="carousel-inner">
          <div className="carousel-item active">
            <Image src={'/images/1.jpeg'} width={1280} height={200} className="d-block" alt={"1.jpeg"}/>
          </div>
          <div className="carousel-item">
            <Image src={'/images/2.jpeg'} width={1280} height={200} className="d-block" alt={"2.jpeg"}/>
          </div>
          <div className="carousel-item">
            <Image src={'/images/3.jpeg'}  width={1280} height={200} className="d-block" alt={"3.jpeg"}/>
          </div>
          <div className="carousel-item">
            <Image src={'/images/4.jpeg'}  width={1280} height={200} className="d-block" alt={"3.jpeg"}/>
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
      <div className="card mb-5 mt-3 mx-2 bg-dark">
        <div className="card-header mx-2 bg-dark border-success"><h3 className='text-center text-red'>Supercar Technology</h3></div>
        <div className="card bg-secondary mx-2">
          <h5 className="card-title mx-1 text-warning mt-2">Defined by success</h5>
          <p className="card-text mb-1 mx-1 text-light">
            The pinnacle of automotive engineering and design, often featuring exceptional speed, powerful engines, advanced aerodynamics, and cutting-edge technology. They are typically characterized by their distinctive and attention-grabbing designs, high price tags, and limited production numbers
            </p>
        </div>
      <div className="card-footer bg-transparent border-success"></div>
</div>

      <Footer/>
    </>
  )
}
