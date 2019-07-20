#[no_mangle]
pub extern fn pearson(x: Vec<f64>, y: Vec<f64>) -> f64 {
    let n: usize = x.len();
    let mut yt: f64;
    let mut xt: f64;
    let mut syy = 0.0;
    let mut sxy = 0.0;
    let mut sxx = 0.0;
    let mut ay = 0.0;
    let mut ax = 0.0;

    for j in 0..n {
        ax = ax + x[j];
        ay = ay + y[j];
    }

    ax = ax / (n as f64);
    ay = ay / (n as f64);

    for j in 0..n {
        xt = x[j] - ax;
        yt = y[j] - ay;
        sxx = sxx + xt * xt;
        syy = syy + yt * yt;
        sxy = sxy + xt * yt;
    }

    // will regularize the unusual case of complete correlation
    let TINY : f64 = 1.0E-20;
    let pcc = sxy / ((sxx * syy).sqrt() + TINY);

    return pcc;
}