mkdir -p build/release
cd build/release
cmake -DLINUX_BUILD=ON ..
make
