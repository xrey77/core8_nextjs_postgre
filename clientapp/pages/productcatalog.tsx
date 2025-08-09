import React, { useEffect, useState } from 'react'
import Image from 'next/image';
import Link from 'next/link';

 const Productcatalog = (props: any) => {
    let [page, setPage] = useState(1);
    let [prods, setProds] = useState([]);
    let [totpage, setTotpage] = useState(null);

    const fetchCatalog = async (pg: any) => {
      await fetch(`/api/product/list?page=${page}`)
      .then((response) => response.json())
      .then((json) => {
        setProds(json.products);
        setTotpage(json.totpages);
      });
    }

    useEffect(() => {
      // fetch(`/api/product/list?page=${page}`)
      // .then((response) => response.json())
      // .then((json) => {
      //   setProds(json.products);
      //   setTotpage(json.totpages);
      //   setPage(json.page);           
      // });

    },[page]);

    const firstPage = (event: any) => {
        event.preventDefault();    
        page = 1;
        setPage(page);
        fetchCatalog(page);
        return;    
      }
    
      const nextPage = (event: any) => {
        event.preventDefault();    
        if (page === totpage) {
            return;
        }
        setPage(page++);
        fetchCatalog(page);
        return;
      }
    
      const prevPage = (event: any) => {
        event.preventDefault();    
        if (page === 1) {
          return;
          }
          setPage(page--);
          fetchCatalog(page);
          return;    
      }
    
      const lastPage = (event: any) => {
        event.preventDefault();
        page = totpage;
        setPage(page);
        fetchCatalog(page);
        return;    
      }

    return(
    <div className="container mb-9">
            <h3 className='text-center'>Product Catalog</h3>
            <div className="card-group mb-3">
            {prods.map((item) => {
                    return (
                    <div key={item.id} className="card">
                        <Image src={item['prod_pic']} className="card-img-top" alt="" width={200} height={200}/>
                        <div className="card-body">
                            <h5 className="card-title">Descriptions</h5>
                            <p className="card-text">{item['descriptions']}</p>
                        </div>
                        <div className="card-footer">
                            <p className="card-text text-danger"><span className="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{item['sell_price']}</strong></p>
                        </div>  
                    </div>
                );
            })}
          </div>    

        <div className='container'>
        <nav aria-label="Page navigation example">
        <ul className="pagination">
          <li className="page-item"><Link onClick={lastPage} className="page-link" href="/#">Last</Link></li>
          <li className="page-item"><Link onClick={prevPage} className="page-link" href="/#">Previous</Link></li>
          <li className="page-item"><Link onClick={nextPage} className="page-link" href="/#">Next</Link></li>
          <li className="page-item"><Link onClick={firstPage} className="page-link" href="/#">First</Link></li>
          <li className="page-item page-link text-danger">Page&nbsp;{page} of&nbsp;{totpage}</li>
        </ul>
      </nav>
      <br/><br/>
      </div>


    </div>
    )
}

export default Productcatalog;