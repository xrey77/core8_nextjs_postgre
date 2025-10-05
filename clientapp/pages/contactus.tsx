'use clinet'
import Footer from './layout/footer';

const Contactus = () => {
    return(
        <div>
            <div className='card bg-primary mt-1 mx-2 border-3 border-white text-light'>
                <div className='card-header border-white'>
                    <h2 className='text-center'>Contact Us</h2>
                </div>
                <p className='card-text mx-2 mt-2'>
                To find contact information for supercars, specifically in the context of the Philippines, you can try contacting Preferred SuperCars directly through their website or by phone. They are located in Miami Beach, Florida. Additionally, you could explore supercar sharing services or look for dealerships or clubs specializing in high-performance vehicles in your area. 
                </p>                    
                <h4 className="card-text text-warning mx-2">
                Here&#39;s a breakdown of how to find contacts:
                </h4>

                <h5 className='card-text text-red mx-2'>
                Preferred SuperCars                    
                </h5>

                <p className='card-text mx-2'>
                They offer a contact form and phone number on their website. 
                </p>

                <h5 className='card-text text-red mx-2'>
                Supercar Sharing Services:                    
                </h5>

                <p className='card-text mx-2'>
                Services like Supercar Sharing may have contact information or dealerships listed for specific brands they offer. 
                </p>

                <h5 className='card-text text-red mx-2'>
                Local Dealerships/Clubs:                    
                </h5>

                <p className='card-text mx-2'>
                Search online for &#34;supercar dealerships&#34; or &#34;supercar clubs&#34; in your region, such as the Philippines, to find relevant contact information. 
                </p>

                <h5 className='card-text text-red mx-2'>
                Specialized Brands:                     
                </h5>

                <p className='card-text mx-2 mb-3'>
                If you have a specific brand in mind (e.g., Lamborghini, Ferrari), you can search for their local dealerships or distributors. 
                </p>

            </div>
            <br/><br/><br/>
            <Footer/>
        </div>
    )
}

export default Contactus;