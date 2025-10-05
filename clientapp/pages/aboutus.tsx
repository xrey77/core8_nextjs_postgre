'use client'
import Footer from './layout/footer';

const Aboutus = () => {
    return(
    <div>
    <div className="card border-3 mt-1 mx-5">
        <div className="card-header bg-success border-white">
            <h2 className="text-center text-light">About Us</h2>
        </div>
        <div className="card-text bg-success">
            <p className="text-light mx-3 mt-2">
            Our primary goals of a supercar company revolve around performance, exclusivity, and financial success. These companies aim to produce vehicles with exceptional speed, handling, and advanced technology, often in limited quantities, to justify high price tags. They also strive to build a strong brand identity and cultivate a loyal customer base.
            </p>

            <p className="text-warning mx-3 mt-2">
            <h4>Performance and Technology:</h4>
            </p>
            <p className="text-info mx-3 mt-2">
                <h5>High-Performance Engineering:</h5>
            </p>
            <p className="text-light mx-3 mt-2">
            Our company focus on pushing the boundaries of automotive engineering to deliver unparalleled performance. This includes powerful engines, advanced aerodynamics, and cutting-edge chassis technology.                
            </p>
            <p className="text-info mx-3 mt-2">
                <h5>Innovation and Technology:</h5>
            </p>
            <p className="text-light mx-3 mt-2">
            Our Supercars often showcase the latest advancements in automotive technology, including hybrid powertrains, advanced driver-assistance systems, and lightweight materials. 
            </p>
            <p className="text-info mx-3 mt-2">
                <h5>High-End Customer Experience: </h5>
            </p>
            <p className="text-light mx-3 mt-2">
            Our Supercar often offer bespoke customization options and personalized service to cater to the discerning tastes of their clientele. 
            </p>
        </div>
    </div>        
    <br/><br/><br/>

    <Footer/>
    </div>    

        );
}

export default Aboutus;