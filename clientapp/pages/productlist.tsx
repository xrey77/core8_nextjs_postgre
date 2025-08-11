'use client'
import Link from 'next/link';
import { useEffect, useState } from 'react'
import axios from 'axios';
import Footer from './layout/footer';

const api = axios.create({
  baseURL: "https://localhost:7292",
  headers: {'Accept': 'application/json',
            'Content-Type': 'application/json'}
})

interface Productdata {
  totpage: string,
  page: string,
  products: Products
}

interface Products {
  id: number,
  descriptions: string,  
  qty: number,
  unit: string,
  sellPrice: number,
  productPicture: string
}

const formatNumberWithCommaDecimal = (number: any) => {
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2, // Ensures at least two decimal places
    maximumFractionDigits: 2, // Limits to two decimal places
  });
  // Format the number
  return formatter.format(number);
};


const Productlist = (props: any) => {

    let [page, setPage] = useState<number>(1);
    let [totpage, setTotpage] = useState<number>(0);
    let [products, setProducts] = useState<Productdata[]>([]);

    const fetchProducts = async (pg: any) => {
      api.get<Productdata>(`/api/listproducts/${pg}`)
      .then((res) => {
        const data: Productdata = res.data;
        setProducts(data.products);
        setTotpage(data.totpage);
        setPage(data.page);
      }, (error: any) => {
              console.log(error.message);
              return;
      });      
    }

    useEffect(() => {
      fetchProducts(page);
   },[page]);

    const firstPage = (event: any) => {
        event.preventDefault();    
        page = 1;
        setPage(page);
        fetchProducts(page);
        return;    
      }
    
      const nextPage = (event: any) => {
        event.preventDefault();    
        if (page === totpage) {
            return;
        }
        setPage(page++);
        fetchProducts(page);
        return;
      }
    
      const prevPage = (event: any) => {
        event.preventDefault();    
        if (page === 1) {
          return;
          }
          setPage(page--);
          fetchProducts(page);
          return;    
      }
    
      const lastPage = (event: any) => {
        event.preventDefault();
        page = totpage;
        setPage(page);
        fetchProducts(page);
        return;    
      }

    return(
    <div className="container">
            <h1>Products List</h1>

            <table className="table">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Descriptions</th>
                <th scope="col">Qty</th>
                <th scope="col">Unit</th>
                <th scope="col">Price</th>
                </tr>
            </thead>
            <tbody>

            {products.map((item) => {
            return (
              <tr key={item.id}>
                 <td>{item['id']}</td>
                 <td>{item['descriptions']}</td>
                 <td>{item['qty']}</td>
                 <td>{item['unit']}</td>
                 <td>&#8369;{formatNumberWithCommaDecimal(item['sellPrice'])}</td>
               </tr>
              );
        })}

            </tbody>
            </table>

            <nav aria-label="Page navigation example">
        <ul className="pagination">
          <li className="page-item"><Link onClick={lastPage} className="page-link" href="/#">Last</Link></li>
          <li className="page-item"><Link onClick={prevPage} className="page-link" href="/#">Previous</Link></li>
          <li className="page-item"><Link onClick={nextPage} className="page-link" href="/#">Next</Link></li>
          <li className="page-item"><Link onClick={firstPage} className="page-link" href="/#">First</Link></li>
          <li className="page-item page-link text-danger">Page&nbsp;{page} of&nbsp;{totpage}</li>

        </ul>
      </nav>
    <Footer/>
  </div>
  )
}
export default Productlist;